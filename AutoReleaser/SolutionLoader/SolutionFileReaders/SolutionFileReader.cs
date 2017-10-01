/*
 * Filename:    SolutionFileReader.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     CommandLine
 * Description: Abstract class used as a base for concrete solution file readers.
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
using System.IO;
using System.Text.RegularExpressions;
using AutoReleaser.SolutionLoader.Configuration;
using AutoReleaser.SolutionLoader.ProjectFileReaders;
using AutoReleaser.SolutionLoader.Util;

namespace AutoReleaser.SolutionLoader.SolutionFileReaders
{
    public abstract class SolutionFileReader : SolutionBrowser
    {
        /// <summary>
        ///     Enumeration with a list of Visual Studio version project files supported.
        /// </summary>
        protected enum SolutionVersion
        {
            VisualStudio2002,
            VisualStudio2003,
            VisualStudio2005,
            VisualStudio2008,
            VisualStudio2010,
            VisualStudio2012,
            VisualStudio2013
        }

        #region Constructors

        protected SolutionFileReader(string solutionFilename, VcbConfiguration configuration) : base(configuration)
        {
            //_projectsNotFound = new List<string>();
            FilesNotFound = new Dictionary<string, string[]>();

            SolutionFilename = solutionFilename;
            var fileContent = FileUtil.ReadTextFile(SolutionFilename);
            // ReSharper disable once VirtualMemberCallInConstructor
            SolutionVersionProp = GetSolutionVersion(fileContent);
            //m_sourceSafeCheckOut = new SourceSafeCommandLine(_solutionFilename, sourceSafeUserOptions);
            // ReSharper disable once VirtualMemberCallInConstructor
            ExtractProjects(fileContent);
        }

        #endregion // Constructors

        #region Protected properties

        /// <summary>
        ///     Gets the root folder of the solution.
        /// </summary>
        protected string SolutionRoot => Path.GetDirectoryName(SolutionFilename);

        #endregion // Protected properties

        #region Implementations of ISolutionBrowser interface

        //public override void CheckOutProjectVersionFiles(ProjectInfo[] projectsToCheckOut) {
        //    Debug.Assert(projectsToCheckOut != null);
        //    if (SourceSafeCommandLine.IsUnderSourceSafeControl(_solutionFilename)) {
        //        m_sourceSafeCheckOut.CheckOut(projectsToCheckOut);
        //    }
        //}

        /// <summary>
        ///     Gets the name of the solution file.
        /// </summary>
        protected string SolutionFilename { get; }

        #endregion // Implementations of ISolutionBrowser interface

        #region Protected methods

        protected virtual void ExtractProjects(string fileContent)
        {
            AllProjects.Clear();
            var regex = new Regex(ProjectPattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            foreach (Match match in regex.Matches(fileContent))
                try
                {
                    var pi = ExtractProjectInfo(match.Value);
                    if (pi.ProjectTypeInfo != ProjectTypeInfo.SetupProject)
                        AllProjects.Add(pi);
                }
                catch (UnknownProjectTypeException)
                {
                }
        }

        #endregion // Protected methods

        #region Public properties

        //public string[] FilesCheckedOut {
        //    get {
        //        return m_sourceSafeCheckOut.FilesCheckedOut;
        //    }
        //}

        //public string[] FilesFailedToCheckOut {
        //    get {
        //        return m_sourceSafeCheckOut.FilesFailedToCheckOut;
        //    }
        //}

        public string[] ProjectsNotFound => new string[0]; //_projectsNotFound.ToArray();

        public Dictionary<string, string[]> FilesNotFound { get; }

        #endregion // Public properties

        #region Protected fields

        //private readonly List<string> _projectsNotFound;

        protected const string UnknownProjectType = "WARNING: \'{0}\' project is of unknown type";

        #endregion // Protected fields

        #region Abstract properties and methods

        protected abstract string ProjectPattern { get; }

        protected SolutionVersion SolutionVersionProp { get; }

        protected abstract SolutionVersion GetSolutionVersion(string fileContent);

        protected abstract ProjectInfo ExtractProjectInfo(string text);

        protected abstract ProjectFileReader GetProjectFileReader(string projectName, string projectFilename, ProjectTypeInfo projectTypeInfo);

        #endregion // Abstract properties and methods
    }
}