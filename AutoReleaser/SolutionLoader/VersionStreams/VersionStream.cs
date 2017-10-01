/*
 * Filename:    VersionStream.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Base class for concrete version streams.
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

using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using AutoReleaser.SolutionLoader.Configuration;
using AutoReleaser.SolutionLoader.Util;
using AutoReleaser.SolutionLoader.Versions;

namespace AutoReleaser.SolutionLoader.VersionStreams
{
    /// <summary>
    ///     Base class for concrete version streams: <c>AssemblyInfoStream</c>,
    ///     <c>ResourceFileStream</c> and <c>SetupVersionStream</c>.
    /// </summary>
    public abstract class VersionStream
    {
        #region Constructors

        protected VersionStream(string filename)
        {
            Debug.Assert(filename != null && File.Exists(filename));
            Filename = filename;
            FileContent = FileUtil.ReadTextFile(filename, out Encoding);
            Debug.Assert(FileContent.Length > 0);
        }

        #endregion // Constructors

        #region Public properties

        public string Filename { get; }

        #endregion // Public properties

        #region Protected methods

        protected ProjectVersion GetVersion(AssemblyVersionType versionType)
        {
            Debug.Assert(versionType != AssemblyVersionType.All);
            var assemblyVersionString = GetVersionString(versionType);
            if (assemblyVersionString.Length == 0)
                return ProjectVersion.Empty;
            var regex = new Regex(VersionPattern);
            var match = regex.Match(assemblyVersionString);
            if (match.Length > 0)
                return new ProjectVersion(match.Value, versionType);
            regex = new Regex(QuotesEnclosedPattern);
            match = regex.Match(assemblyVersionString);
            return new ProjectVersion(match.ToString().Trim('\"'), versionType);
        }

        #endregion // Protected methods

        #region Abstract methods and properties

        public abstract AssemblyVersions GetVersions();

        public abstract void SaveVersion(AssemblyVersionType typeToSave, string newVersion);

        protected abstract string GetVersionString(AssemblyVersionType versionType);

        protected abstract string VersionPattern { get; }

        #endregion // Abstract methods

        #region Protected fields

        protected string FileContent;

        protected readonly Encoding Encoding;

        protected const string StartOfLine = "^\\s*";
        protected const string OneOrMoreWhitespacePattern = "\\s+";
        protected const string OptionalWhitespacePattern = "\\s*";
        protected const string QuotesEnclosedPattern = "\"(?>[^\"]+)*\"";

        #endregion // Protected fields
    }
}