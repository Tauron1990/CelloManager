/*
 * Filename:    ResourceFileStream.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Reads and writes versions from/to VC++ resource files.
 * Copyright:   Julijan Šribar, 2004-2013
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty.  In no event will the author(s) be held liable for any damages
 * arising from the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using AutoReleaser.SolutionLoader.Configuration;
using AutoReleaser.SolutionLoader.Util;
using AutoReleaser.SolutionLoader.VersionStreams;

namespace AutoReleaser.SolutionLoader.Versions
{
    /// <summary>
    ///     Class responsible for getting and saving version from VC++ resource
    ///     (.RC) files.
    /// </summary>
    public class ResourceFileStream : VersionStream
    {
        private const string VsVersionInfoHeader = "VS_VERSION_INFO";
        private const string VersionInfoHeader = "VERSIONINFO";
        private const string VersionInfoHeaderLine = StartOfLine + VsVersionInfoHeader + OneOrMoreWhitespacePattern + VersionInfoHeader;
        private const string FileVersionCaps = "FILEVERSION";
        private const string ProductVersionCaps = "PRODUCTVERSION";
        private const string Value = "VALUE";
        private const string FileVersionQuoted = "\"FileVersion\"";
        private const string ProductVersionQuoted = "\"ProductVersion\"";

        private const string ResourceVersionPattern = @"([0-9]+)([\,\.]\s*[0-9]+){1,3}";
        //private const string VersionPatternQuoted = "\"" + OptionalWhitespacePattern + ResourceVersionPattern + OptionalWhitespacePattern + "\"";

        /// <summary>
        ///     Creates <c>ResourceFileStream</c> object for resource file provided.
        /// </summary>
        /// <param name="filename">
        ///     Name of the resource file.
        /// </param>
        public ResourceFileStream(string filename) : base(filename)
        {
        }

        protected override string VersionPattern => ResourceVersionPattern;

        /// <summary>
        ///     Reads version from the file.
        /// </summary>
        /// <returns>
        ///     <c>AssemblyVersions</c> collection.
        /// </returns>
        public override AssemblyVersions GetVersions()
        {
            var projectVersions = new Dictionary<AssemblyVersionType, ProjectVersion>(AssemblyVersions.AssemblyVersionTypes.Length);
            foreach (var avt in AssemblyVersions.AssemblyVersionTypes) // resource file does not contain assembly version
                if (avt == AssemblyVersionType.AssemblyVersion) projectVersions[avt] = ProjectVersion.Empty;
                else projectVersions[avt] = GetVersion(avt);
            return new AssemblyVersions(projectVersions[AssemblyVersionType.AssemblyVersion], projectVersions[AssemblyVersionType.AssemblyFileVersion],
                projectVersions[AssemblyVersionType.AssemblyInformationalVersion]);
        }

        /// <summary>
        ///     Saves version to resource file.
        /// </summary>
        /// <param name="typeToSave">
        ///     Flags specifying which versions to save.
        /// </param>
        /// <param name="newVersion">
        ///     New version to save.
        /// </param>
        public override void SaveVersion(AssemblyVersionType typeToSave, string newVersion)
        {
            Debug.Assert(typeToSave != AssemblyVersionType.All);
            if (typeToSave == AssemblyVersionType.AssemblyVersion)
                return;
            if (typeToSave == AssemblyVersionType.AssemblyFileVersion)
            {
                SetHeaderVersionString(FileVersionCaps, newVersion);
                SetBlockVersionString(FileVersionQuoted, newVersion);
            }
            if (typeToSave == AssemblyVersionType.AssemblyInformationalVersion)
            {
                SetHeaderVersionString(ProductVersionCaps, newVersion);
                SetBlockVersionString(ProductVersionQuoted, newVersion);
            }
            FileUtil.SaveTextFile(Filename, FileContent, Encoding);
        }

        /// <summary>
        ///     Extracts version string from resource file content.
        /// </summary>
        /// <param name="versionType">
        ///     Version type to search. Can be AssemblyFileVersion or
        ///     AssemblyInformationalVersion, corresponding to FILEVERSION and
        ///     PRODUCTVERSION respectively.
        /// </param>
        /// <returns>
        ///     Version string found.
        /// </returns>
        protected override string GetVersionString(AssemblyVersionType versionType)
        {
            Debug.Assert(versionType != AssemblyVersionType.AssemblyVersion && versionType != AssemblyVersionType.All);
            // if VS_VERSION_INFO header not found, there is no valid version
            var regex = new Regex(VersionInfoHeaderLine, RegexOptions.Multiline);
            var match = regex.Match(FileContent);
            if (!match.Success)
                return "";
            var offset = match.Index + match.Length;
            var pattern = "";
            switch (versionType)
            {
                case AssemblyVersionType.AssemblyFileVersion:
                    pattern = StartOfLine + FileVersionCaps + OneOrMoreWhitespacePattern + VersionPattern;
                    break;
                case AssemblyVersionType.AssemblyInformationalVersion:
                    pattern = StartOfLine + ProductVersionCaps + OneOrMoreWhitespacePattern + VersionPattern;
                    break;
                default:
                    Debug.Assert(false, $"Illegal versionName: {versionType.ToString()}");
                    break;
            }
            regex = new Regex(pattern, RegexOptions.Multiline);
            match = regex.Match(FileContent, offset);
            Debug.Assert(match.Value.Length > 0);
            return match.Value;
        }

        private void SetHeaderVersionString(string versionName, string version)
        {
            // first find start of the header
            var regex = new Regex(VersionInfoHeaderLine, RegexOptions.Multiline);
            var match = regex.Match(FileContent);
            regex = new Regex(StartOfLine + versionName + OneOrMoreWhitespacePattern + ResourceVersionPattern, RegexOptions.Multiline);
            FindAndReplaceAllVersionStrings(regex, version, match.Index + match.Length);
        }


        private void SetBlockVersionString(string atributeName, string version)
        {
            // first find start of the header
            var regex = new Regex(VersionInfoHeaderLine, RegexOptions.Multiline);
            var match = regex.Match(FileContent);
            regex = new Regex(StartOfLine + Value + OneOrMoreWhitespacePattern + atributeName + "\\," + OptionalWhitespacePattern + "\"" + VersionPattern + "\"", RegexOptions.Multiline);
            // in block, versions may be shorter 
            version = ReduceBlockVersion(regex, version, match.Index + match.Length);
            FindAndReplaceAllVersionStrings(regex, version, match.Index + match.Length);
        }

        /// <summary>
        ///     Block FileVersion and ProductVersion may consist of less than 4
        ///     numbers, and in such case version to be applied should be trimmed.
        /// </summary>
        /// <param name="regularExpression">
        ///     Regular expression object for the block version line.
        /// </param>
        /// <param name="version">
        ///     Version to be applied.
        /// </param>
        /// <param name="offset">
        ///     Offset from file start.
        /// </param>
        /// <returns>
        ///     Trimmed version that will be applied.
        /// </returns>
        private string ReduceBlockVersion(Regex regularExpression, string version, int offset)
        {
            var line = regularExpression.Match(FileContent, offset).Value;
            var regex = new Regex(VersionPattern);
            var currVersion = regex.Match(line).Value;
            var versionLength = currVersion.Split('.', ',').Length;
            if (versionLength == 4)
                return version;
            offset = 0;
            while (versionLength > 0)
            {
                offset = version.IndexOf(".", offset + 1, StringComparison.Ordinal);
                versionLength--;
            }
            return version.Substring(0, offset);
        }


        private void FindAndReplaceAllVersionStrings(Regex regularExpression, string version, int offset)
        {
            var commaSeparatedVersion = version.Replace('.', ',');
            var regexVersion = new Regex(VersionPattern);
            var lineMatch = regularExpression.Match(FileContent, offset);
            while (lineMatch.Index > 0)
            {
                // check the separator used for version components
                var newVersion = regexVersion.Match(lineMatch.Value).Value;
                newVersion = regexVersion.Replace(lineMatch.Value, newVersion.IndexOf(',') != -1 ? commaSeparatedVersion : version);

                FileContent = regularExpression.Replace(FileContent, newVersion, 1, lineMatch.Index);
                lineMatch = regularExpression.Match(FileContent, lineMatch.Index + lineMatch.Length);
            }
        }
    }
}