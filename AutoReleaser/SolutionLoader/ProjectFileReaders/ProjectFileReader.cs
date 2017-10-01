/*
 * Filename:    ProjectFileReader.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     CommandLine
 * Description: Abstract class inhereted by concrete reader classes (e.g. 
 *              DspFileReader, EtpFileReader, SetupProjectFileReader, 
 *              VS7ProjectFileReader). 
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AutoReleaser.SolutionLoader.Util;
using AutoReleaser.SolutionLoader.Versions;
using AutoReleaser.SolutionLoader.VersionStreams;

namespace AutoReleaser.SolutionLoader.ProjectFileReaders
{
    /// <summary>
    ///     Abstract class subclassed by concrete reader classes (<c>DspFileReader</c>,
    ///     <c>EtpFileReader</c>, <c>SetupProjectFileReader</c>, <c>VS7ProjectFileReader</c>,
    ///     <c>VS8ProjectFileReader</c>).
    /// </summary>
    public abstract class ProjectFileReader
    {
        #region Public properties

        public ProjectInfo ProjectInfo => GetProjectInfo();

        #endregion // Public properties

        #region Protected methods

        private ProjectInfo GetProjectInfo()
        {
            var versionStreams = new ArrayList();
            // AssemblyVersions used to find the largest one
            var assemblyVersions = AssemblyVersions.Empty;
            var assemblyInfoFilename = GetProjectVersionFile();
            //if (_projectTypeInfo.ProjectType == ProjectType.SetupProject)
            //{
            //    VersionStream versionStream = new SetupVersionStream(assemblyInfoFilename);
            //    AssemblyVersions setupVersion = versionStream.GetVersions();
            //    // setup projects are always at solution root
            //    return new ProjectInfo(ProjectName, ProjectFilename, _projectTypeInfo, false, Level, setupVersion, new VersionStream[] { versionStream });
            //}
            //if (_projectTypeInfo.ProjectType == ProjectType.InstallShieldLEProject)
            //{
            //    VersionStream versionStream = new InstallShieldLEVersionStream(assemblyInfoFilename);
            //    AssemblyVersions setupVersion = versionStream.GetVersions();
            //    // setup projects are always at solution root
            //    return new ProjectInfo(ProjectName, ProjectFilename, _projectTypeInfo, false, Level, setupVersion, new VersionStream[] { versionStream });
            //}
            // for a VC++ project search for resource files which may contain version if configured so
            if (_projectTypeInfo.ProjectType == ProjectType.VCppProject)
            {
                var resourceFilenames = GetResourceFilenames();
                // if VC++ project contains both AssemblyInfo file and resource 
                // file with version, compare them and get the larger value
                Debug.Assert(resourceFilenames != null);
                foreach (var resourceFilename in resourceFilenames)
                {
                    VersionStream resourceFileStream = new ResourceFileStream(resourceFilename);
                    var resourceVersion = resourceFileStream.GetVersions();
                    versionStreams.Add(resourceFileStream);
                    assemblyVersions = AssemblyVersions.Max(assemblyVersions, resourceVersion);
                }
            }
            if (assemblyInfoFilename != null)
            {
                VersionStream assemblyInfoStream = new AssemblyInfoStream(assemblyInfoFilename);
                var assemblyInfoVersions = assemblyInfoStream.GetVersions();
                versionStreams.Add(assemblyInfoStream);
                assemblyVersions = assemblyVersions == AssemblyVersions.Empty ? assemblyInfoVersions : AssemblyVersions.Max(assemblyVersions, assemblyInfoVersions);
            }
            var vs = (VersionStream[]) versionStreams.ToArray(typeof(VersionStream));
            var isProjectModified = IsProjectModified(vs);
            return new ProjectInfo(_projectName, ProjectFilename, _projectTypeInfo, isProjectModified, _level, assemblyVersions, vs);
        }

        #endregion // Protected methods

        #region Constructors

        /// <summary>
        ///     Initializes empty <c>ProjectFileReader</c> object.
        /// </summary>
        private ProjectFileReader()
        {
            MissingFiles = new List<string>();
        }

        /// <summary>
        ///     Initializes <c>ProjectFileReader</c> object for the project.
        /// </summary>
        /// <param name="projectName">
        ///     Name of the project.
        /// </param>
        /// <param name="projectFilename">
        ///     Full name of the project file.
        /// </param>
        /// <param name="projectTypeInfo">
        ///     <c>ProjectTypeInfo</c> identifying the type of the project.
        /// </param>
        /// <param name="level">
        ///     Level in solution tree hierarchy.
        /// </param>
        protected ProjectFileReader(string projectName, string projectFilename, ProjectTypeInfo projectTypeInfo, int level)
            : this()
        {
            _projectName = projectName;
            ProjectFilename = projectFilename;
            _projectTypeInfo = projectTypeInfo;
            _level = level;
            // ReSharper disable once VirtualMemberCallInConstructor
            _includedFiles = GetIncludedFiles();
        }

        #endregion // Constructors

        #region Protected properties

        public string[] FilesNotFound => MissingFiles.ToArray();

        protected abstract string[] GetIncludedFiles();

        #endregion // Protected properties

        #region Private methods

        /// <summary>
        ///     Returns names of all resource files with version.
        /// </summary>
        /// <returns>
        ///     Array of resource filenames.
        /// </returns>
        private string[] GetResourceFilenames()
        {
            Debug.Assert(_projectTypeInfo.ProjectType == ProjectType.VCppProject);
            var resourceFiles = new ArrayList();

            foreach (var filename in from filename in _includedFiles
                where FileUtil.FilenameMatchesPattern(filename, "*.rc")
                let rfs = new ResourceFileStream(filename)
                where !rfs.GetVersions().Equals(AssemblyVersions.Empty)
                select filename)
                resourceFiles.Add(filename);

            return (string[]) resourceFiles.ToArray(typeof(string));
        }

        /// <summary>
        ///     Returns version filename.
        /// </summary>
        /// <returns>
        ///     Name of the version file.
        /// </returns>
        private string GetProjectVersionFile()
        {
            foreach (var filename in _includedFiles)
                if (FileUtil.FilenameMatchesPattern(filename, _projectTypeInfo.AssemblyInfoFilename))
                    return filename;
            return null;
        }

        /// <summary>
        ///     Compares date &amp; time stamps for files in the project with
        ///     version files.
        /// </summary>
        /// <param name="versionStreams">
        ///     Array of <c>VersionStream</c> objects used as a reference for
        ///     comparison.
        /// </param>
        /// <returns>
        ///     <c>true</c> if any file has a more recent time stamp than version
        ///     files. Otherwise <c>false</c>.
        /// </returns>
        private bool IsProjectModified(VersionStream[] versionStreams)
        {
            Debug.Assert(versionStreams != null);
            var lastWriteTime = DateTime.MinValue;
            string referenceVersionFile = null;
            foreach (var vs in versionStreams)
            {
                var fileWriteTime = FileUtil.GetLastWriteTime(vs.Filename);
                if (fileWriteTime <= lastWriteTime) continue;

                lastWriteTime = fileWriteTime;
                referenceVersionFile = vs.Filename;
            }
            // check if the any file in the project has been modified later than reference file
            return referenceVersionFile != null && IsAnyFileInProjectNewer(referenceVersionFile);
        }

        /// <summary>
        ///     Checks if any file in the project has a more recent date &amp;
        ///     time stamp compared to file containing version.
        /// </summary>
        /// <param name="referenceFile">
        ///     Reference version file.
        /// </param>
        /// <returns>
        ///     Returns <c>true</c> if any file has a more recent date &amp; time
        ///     stamp than the reference file.
        /// </returns>
        private bool IsAnyFileInProjectNewer(string referenceFile)
        {
            var dateTimeToCompare = FileUtil.GetLastWriteTime(referenceFile);
            foreach (var file in _includedFiles)
                if (!FileUtil.PathsAreEqual(file, referenceFile))
                {
                    var comp = dateTimeToCompare.CompareTo((object) FileUtil.GetLastWriteTime(file));
                    if (comp <= 0)
                        return true;
                }
            return false;
        }

        #endregion // Private methods

        #region Protected fields

        private readonly string _projectName;

        protected readonly string ProjectFilename;

        private readonly ProjectTypeInfo _projectTypeInfo;

        private readonly int _level;

        private readonly string[] _includedFiles;

        protected const string FileTag = "File";

        protected readonly List<string> MissingFiles;

        #endregion // Protected fields
    }
}