/*
 * Filename:    AssemblyInfoStream.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Reads and writes assembly versions from/to assembly info files.
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

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using AutoReleaser.SolutionLoader.Configuration;
using AutoReleaser.SolutionLoader.Util;
using AutoReleaser.SolutionLoader.VersionStreams;

namespace AutoReleaser.SolutionLoader.Versions
{
    /// <summary>
    ///     Class for reading and writing assembly versions from assembly info files.
    /// </summary>
    public class AssemblyInfoStream : VersionStream
    {
        #region VersionPatternProvider

        /// <summary>
        ///     Class responsible for building assembly version search patterns
        ///     for different project types.
        /// </summary>
        private class VersionPatternProvider
        {
            /// <summary>
            ///     Table with opening and closing brackets for different file
            ///     types.
            /// </summary>
            private static readonly Dictionary<string, string[]> Brackets = new Dictionary<string, string[]>
            {
                {".cs", new[] {"\\[", "\\]"}},
                {".vb", new[] {"\\<", "\\>"}},
                {".cpp", new[] {"\\[", "\\]"}},
                {".jsl", new[] {"/\\*\\*", "\\*/"}}
            };

            /// <summary>
            ///     Table with assembly prefixes for different file types.
            /// </summary>
            private static readonly Dictionary<string, string> AssemblyPrefixes = new Dictionary<string, string>
            {
                {".cs", "\\s*assembly\\s*:\\s*"},
                {".vb", "Assembly\\s*:\\s*"},
                {".cpp", "\\s*assembly\\s*:\\s?"},
                {".jsl", "\\s*@assembly\\s*"}
            };

            /// <summary>
            ///     File extension for handled assembly info file.
            /// </summary>
            private readonly string _extension;

            static VersionPatternProvider()
            {
            }

            public VersionPatternProvider(string assemblyInfoFileExtension)
            {
                _extension = assemblyInfoFileExtension;
            }

            /// <summary>
            ///     Gets string used as opening bracket for attribute.
            /// </summary>
            public string LeftBracket => Brackets[_extension][0];

            /// <summary>
            ///     Gets string used as a closing bracket for attribute.
            /// </summary>
            public string RightBracket => Brackets[_extension][1];

            /// <summary>
            ///     Gets string which prefixes attribute name
            /// </summary>
            public string AssemblyPrefix => AssemblyPrefixes[_extension];
        }

        #endregion // VersionPatternProvider

        private const string ParenthesesEnclosedString = OptionalWhitespacePattern + @"\((?>[^()]+)*\)" + OptionalWhitespacePattern;

        private const string Attribute = "Attribute";

        //public const string AssemblyVersion = "AssemblyVersion";
        //public const string AssemblyVersionAttribute = "AssemblyVersionAttribute";

        //public const string AssemblyFileVersion = "AssemblyFileVersion";
        //public const string AssemblyFileVersionAttribute = "AssemblyFileVersionAttribute";

        //public const string AssemblyInformationalVersion = "AssemblyInformationalVersion";
        //public const string AssemblyInforationalVersionAttribute = "AssemblyInformationalVersionAttribute";

        private const string AssemblyInfoVersionPattern = @"([0-9]+)((\.[0-9]+)*)(((\.\*)|((\.[0-9]+)*)((\.[0-9]+)*)))";

        private readonly VersionPatternProvider _versionPatternProvider;

        public AssemblyInfoStream(string fileName) : base(fileName)
        {
            _versionPatternProvider = new VersionPatternProvider(Path.GetExtension(fileName));
        }


        protected override string VersionPattern => AssemblyInfoVersionPattern;

        public override AssemblyVersions GetVersions()
        {
            var projectVersions = new Dictionary<AssemblyVersionType, ProjectVersion>(AssemblyVersions.AssemblyVersionTypes.Length);
            foreach (var avt in AssemblyVersions.AssemblyVersionTypes) projectVersions[avt] = GetVersion(avt);
            return new AssemblyVersions(projectVersions[AssemblyVersionType.AssemblyVersion], projectVersions[AssemblyVersionType.AssemblyFileVersion],
                projectVersions[AssemblyVersionType.AssemblyInformationalVersion]);
        }

        public override void SaveVersion(AssemblyVersionType typeToSave, string newVersion)
        {
            SetVersionString(typeToSave, newVersion);
            FileUtil.SaveTextFile(Filename, FileContent, Encoding);
        }

        protected override string GetVersionString(AssemblyVersionType versionType)
        {
            Debug.Assert(versionType != AssemblyVersionType.All);
            var shortAttributeName = versionType.ToString();
            Debug.Assert(shortAttributeName != null && !shortAttributeName.EndsWith(Attribute));
            // first try with a short name
            var pattern = BuildAttributeLinePattern(shortAttributeName);
            var regex = new Regex(pattern, RegexOptions.Multiline);
            var match = regex.Match(FileContent);
            if (match.Length > 0)
                return match.ToString();
            // now try with a long one
            pattern = BuildAttributeLinePattern(shortAttributeName + Attribute);
            regex = new Regex(pattern, RegexOptions.Multiline);
            match = regex.Match(FileContent);
            return match.ToString();
        }

        /// <summary>
        ///     Builds a pattern to search the entire attribute line.
        /// </summary>
        /// <param name="attributeName">
        ///     Attribute name to search for
        /// </param>
        /// <returns>
        ///     Pattern for search.
        /// </returns>
        private string BuildAttributeLinePattern(string attributeName)
        {
            var leftBracket = _versionPatternProvider.LeftBracket;
            var rightBracket = _versionPatternProvider.RightBracket;
            var assemblyPrefix = _versionPatternProvider.AssemblyPrefix;
            var pattern = new StringBuilder(StartOfLine);
            pattern.Append(leftBracket);
            pattern.Append(assemblyPrefix);
            pattern.Append(attributeName);
            pattern.Append(ParenthesesEnclosedString);
            pattern.Append(rightBracket);
            return pattern.ToString();
        }

        private void SetVersionString(AssemblyVersionType versionType, string newVersion)
        {
            var entireAttributeString = GetVersionString(versionType);
            Debug.Assert(entireAttributeString.Length > 0);
            var regex = new Regex(QuotesEnclosedPattern);
            var newVersionString = regex.Replace(entireAttributeString, string.Format(CultureInfo.CurrentCulture, "\"{0}\"", newVersion), 1);

            FileContent = FileContent.Replace(entireAttributeString, newVersionString);
        }
    }
}