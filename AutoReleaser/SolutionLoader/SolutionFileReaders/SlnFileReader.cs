/*
 * Filename:    SlnFileReader.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     CommandLine
 * Description: Base class for concrete Visual Studio .NET solution readers.
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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AutoReleaser.SolutionLoader.Configuration;
using AutoReleaser.SolutionLoader.Util;

namespace AutoReleaser.SolutionLoader.SolutionFileReaders
{
    /// <summary>
    ///     Reads the content of SLN file and gets a list of project files.
    /// </summary>
    public abstract class SlnFileReader : SolutionFileReader
    {
        #region SlnFileReaderFactory

        public static class SlnFileReaderFactory
        {
            public static SlnFileReader GetSlnFileReader(string solutionFilename, VcbConfiguration configuration)
            {
                Debug.Assert(File.Exists(solutionFilename) && solutionFilename.ToLower().EndsWith(".sln"));
                var fileContent = FileUtil.ReadTextFile(solutionFilename).TrimStart(null);

                foreach (var pair in from pair in HeaderFormatVersions let version = pair.Value let pattern = $"{SolutionFileHeader} {version}" where fileContent.StartsWith(pattern) select pair)
                    switch (pair.Key)
                    {
                        case SolutionVersion.VisualStudio2005:
                        case SolutionVersion.VisualStudio2008:
                        case SolutionVersion.VisualStudio2010:
                        case SolutionVersion.VisualStudio2012:
                        case SolutionVersion.VisualStudio2013:
                            return new Vs8SlnFileReader(solutionFilename, configuration);
                        default:
                            Debug.Assert(false, "Not supported file format");
                            break;
                    }
                throw new InvalidFileFormatException(solutionFilename);
            }
        }

        #endregion // SlnFileReaderFactory

        protected const string GuidPattern = "\\{[0-9,a-f]{8}-[0-9,a-f]{4}-[0-9,a-f]{4}-[0-9,a-f]{4}-[0-9,a-f]{12}\\}";
        private const string GuidQuotedPattern = "\\\"" + GuidPattern + "\\\"";
        private const string ProjectNamePattern = ".+";
        private const string ProjectNameQuotedPattern = "\\\"" + ProjectNamePattern + "\\\"";
        private const string ProjectFilePattern = ".+";
        private const string ProjectFileQuotedPattern = "\\\"" + ProjectFilePattern + "\\\"";

        private const string ProjectPattern2002 = "Project\\(" + GuidQuotedPattern + "\\)\\s*=\\s*" + ProjectNameQuotedPattern + ",\\s*" + ProjectFileQuotedPattern + ",\\s*" + GuidQuotedPattern + "\\s*$";
        private const string ProjectPattern2003 = "Project\\(" + GuidQuotedPattern + "\\)\\s*=\\s*" + ProjectNameQuotedPattern + ",\\s*" + ProjectFileQuotedPattern + ",\\s*" + GuidQuotedPattern + "\\s*$";
        private const string ProjectInfoPattern = "\\(" + GuidQuotedPattern + "\\)\\s*=\\s*" + ProjectNameQuotedPattern + ",\\s*" + ProjectFileQuotedPattern + ",\\s*" + GuidQuotedPattern;

        private const string SolutionFileHeader = "Microsoft Visual Studio Solution File, Format Version";

        private static readonly Dictionary<SolutionVersion, string> HeaderFormatVersions;

        static SlnFileReader()
        {
            HeaderFormatVersions = new Dictionary<SolutionVersion, string>
            {
                {SolutionVersion.VisualStudio2002, "7.00"},
                {SolutionVersion.VisualStudio2003, "8.00"},
                {SolutionVersion.VisualStudio2005, "9.00"},
                {SolutionVersion.VisualStudio2008, "10.00"},
                {SolutionVersion.VisualStudio2010, "11.00"},
                {SolutionVersion.VisualStudio2012, "12.00"},
                {SolutionVersion.VisualStudio2013, "12.00"}
            };
        }

        protected SlnFileReader(string solutionFilename, VcbConfiguration configuration) : base(solutionFilename, configuration)
        {
        }

        /// <summary>
        ///     Gets pattern used to identify individual project information.
        /// </summary>
        protected override string ProjectPattern
        {
            get
            {
                switch (SolutionVersionProp)
                {
                    case SolutionVersion.VisualStudio2002:
                    case SolutionVersion.VisualStudio2005:
                    case SolutionVersion.VisualStudio2008:
                    case SolutionVersion.VisualStudio2010:
                    case SolutionVersion.VisualStudio2012:
                    case SolutionVersion.VisualStudio2013:
                        return ProjectPattern2002;
                    case SolutionVersion.VisualStudio2003:
                        return ProjectPattern2003;
                }
                Debug.Assert(false, $"Not supported solution (.sln) file version {SolutionVersionProp}");
                return "";
            }
        }

        /// <summary>
        ///     Reads file header to check file validity and get solution version.
        /// </summary>
        /// <returns>
        ///     <c>SolutionVersion</c> of the corresponding file.
        /// </returns>
        protected override SolutionVersion GetSolutionVersion(string fileContent)
        {
            Debug.Assert(fileContent != null && SolutionFilename.Length > 0);

            foreach (var pair in from pair in HeaderFormatVersions
                let version = pair.Value
                let pattern = $"\\s*{SolutionFileHeader} {version.Replace(".", "\\.")}\\s*$"
                let regex = new Regex(pattern, RegexOptions.Multiline)
                where regex.IsMatch(fileContent)
                select pair)
                return pair.Key;

            throw new InvalidFileFormatException(SolutionFilename);
        }

        protected override ProjectInfo ExtractProjectInfo(string text)
        {
            return ExtractProjectInfo(text, out _);
        }

        /// <summary>
        ///     Extracts <c>ProjectInfo</c> from the project file.
        /// </summary>
        /// <param name="text">
        ///     Content of the project file used for extraction.
        /// </param>
        /// <param name="guid"></param>
        /// <returns>
        ///     <c>ProjectInfo</c> containing requested information.
        /// </returns>
        protected ProjectInfo ExtractProjectInfo(string text, out string guid)
        {
            var regex = new Regex(ProjectInfoPattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            var match = regex.Match(text);
            Debug.Assert(match.Success);
            var strings = match.Value.Split('=', ',');
            // extract CLSID and GUID
            var regexGuid = new Regex(GuidPattern, RegexOptions.IgnoreCase);
            var matchGuid = regexGuid.Match(strings[0]);
            Debug.Assert(matchGuid.Success);
            var clsid = matchGuid.Value;
            matchGuid = regexGuid.Match(strings[3]);
            Debug.Assert(matchGuid.Success);
            guid = matchGuid.Value;
            var projectName = strings[1].Trim().Trim('\"');
            Debug.Assert(projectName.Length > 0);
            var projectFilename = strings[2].Trim().Trim('\"');
            Debug.Assert(projectFilename.Length > 0);
            if (projectFilename.StartsWith(InetRootLocator.Localhost) && InetRootLocator.Instance.IsIisAvailable)
                projectFilename = InetRootLocator.Instance.GetLocalPath(projectFilename);
            else if (projectFilename.StartsWith(InetRootLocator.Http))
                return new ProjectInfo(projectName, projectFilename, ProjectTypeInfo.FileBasedWebProject, 0);
            else
                projectFilename = FileUtil.CombinePaths(SolutionRoot, projectFilename);
            var pti = ProjectTypeInfo.ProjectTypeInfos[clsid];
            if (pti == null)
                throw new UnknownProjectTypeException(projectName, clsid);
            if (!pti.IsVersionable)
                //if (pti == ProjectTypeInfo.SolutionFolder || pti == ProjectTypeInfo.DatabaseProject || pti == ProjectTypeInfo.FileBasedWebProject)
                return new ProjectInfo(projectName, projectFilename, pti, 0);
            var pfr = GetProjectFileReader(projectName, projectFilename, pti);
            if (pfr.FilesNotFound.Length > 0)
                FilesNotFound.Add(projectName, pfr.FilesNotFound);
            return pfr.ProjectInfo;
        }
    }
}