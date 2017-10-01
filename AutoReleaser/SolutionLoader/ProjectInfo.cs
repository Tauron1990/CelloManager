/*
 * Filename:    ProjectInfo.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Class with project project name, current and next version.
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
using System.Windows;
using AutoReleaser.SolutionLoader.Configuration;
using AutoReleaser.SolutionLoader.Resources;
using AutoReleaser.SolutionLoader.Versions;
using AutoReleaser.SolutionLoader.VersionStreams;

namespace AutoReleaser.SolutionLoader
{
    public class ProjectComparer : IComparer<ProjectInfo>
    {
        public int Compare(ProjectInfo pi1, ProjectInfo pi2)
        {
            if (pi1 == null)
            {
                if (pi2 == null) return 0;
                return -1;
            }
            if (pi2 == null) return 1;

            if (pi1.ProjectTypeInfo.ProjectType != pi2.ProjectTypeInfo.ProjectType)
            {
                var firstTypePriority = ProjectTypeCompare(pi1.ProjectTypeInfo.ProjectType);
                if (firstTypePriority != 0)
                    return firstTypePriority;
                var secondTypePriority = ProjectTypeCompare(pi2.ProjectTypeInfo.ProjectType);
                if (secondTypePriority != 0)
                    return -secondTypePriority;
            }
            var projectName1 = pi1.ProjectName;
            var projectName2 = pi2.ProjectName;
            return string.CompareOrdinal(projectName1, projectName2);
        }

        private static int ProjectTypeCompare(ProjectType projectType)
        {
            switch (projectType)
            {
                case ProjectType.SolutionFolder:
                    return -1;
                case ProjectType.SetupProject:
                    return +1;
            }
            return 0;
        }
    }

    /// <summary>
    ///     ProjectInfo class contains project name, current version and next
    ///     version.
    /// </summary>
    public class ProjectInfo
    {
        private static readonly string TxtSaveErrorTitle;
        private static readonly string TxtCannotSaveFile;

        /// <summary>
        ///     Flags indicating which assembly versions require version increment.
        /// </summary>
        private readonly IDictionary<AssemblyVersionType, bool> _assemblyVersionsUpdateInfo;

        /// <summary>
        ///     List of all version streams attached to this project.
        /// </summary>
        private readonly List<VersionStream> _versionStreams;

        /// <summary>
        ///     Current AssemblyVersion of the project.
        /// </summary>
        public readonly AssemblyVersions CurrentAssemblyVersions;

        /// <summary>
        ///     Full name of the project file.
        /// </summary>
        public readonly string FullName;

        /// <summary>
        ///     Flag indicating that project has been modifed.
        /// </summary>
        public readonly bool Modified;

        /// <summary>
        ///     Project name.
        /// </summary>
        public readonly string ProjectName;

        /// <summary>
        ///     Information related to type of the project.
        /// </summary>
        public readonly ProjectTypeInfo ProjectTypeInfo;

        /// <summary>
        ///     Sorted list of subprojects.
        /// </summary>
        public readonly SortedSet<ProjectInfo> SubProjects;

        /// <summary>
        ///     Type constructor. Initializes localized strings.
        /// </summary>
        static ProjectInfo()
        {
            var resources = Shared.ResourceManager;
            Debug.Assert(resources != null);

            TxtSaveErrorTitle = resources.GetString("Save Error");
            TxtCannotSaveFile = resources.GetString("Cannot save file");

            Debug.Assert(TxtSaveErrorTitle != null);
            Debug.Assert(TxtCannotSaveFile != null);
        }

        /// <summary>
        ///     Hides default constructor.
        /// </summary>
        private ProjectInfo()
        {
            _assemblyVersionsUpdateInfo = new Dictionary<AssemblyVersionType, bool>(AssemblyVersions.AssemblyVersionTypes.Length)
            {
                [AssemblyVersionType.AssemblyVersion] = false,
                [AssemblyVersionType.AssemblyFileVersion] = false,
                [AssemblyVersionType.AssemblyInformationalVersion] = false
            };
            _versionStreams = new List<VersionStream>();
            SubProjects = new SortedSet<ProjectInfo>(new ProjectComparer());
            Modified = false;
            Level = 0;
            CurrentAssemblyVersions = AssemblyVersions.Empty;
            ToBecomeAssemblyVersions = AssemblyVersions.Empty;
        }


        public ProjectInfo(string projectName, string projectFullName, ProjectTypeInfo projectTypeInfo, bool modified, int level, AssemblyVersions assemblyVersions, VersionStream[] versionStreams)
            : this()
        {
            CurrentAssemblyVersions = assemblyVersions;
            _versionStreams = new List<VersionStream>(versionStreams);

            Modified = modified;
            ProjectName = projectName;
            FullName = projectFullName;
            ProjectTypeInfo = projectTypeInfo;
            Level = level;
        }

        public ProjectInfo(string projectName, string projectFullName, ProjectTypeInfo projectTypeInfo, int level) : this(projectName, projectFullName, projectTypeInfo, false, level, AssemblyVersions.Empty, new VersionStream[0])
        {
        }

        /// <summary>
        ///     Gets the indentation level.
        /// </summary>
        private int Level { get; set; }

        /// <summary>
        ///     Gets the flag indicating if version update will be done.
        /// </summary>
        public bool ToUpdate => _assemblyVersionsUpdateInfo[AssemblyVersionType.AssemblyVersion] | _assemblyVersionsUpdateInfo[AssemblyVersionType.AssemblyFileVersion] |
                                _assemblyVersionsUpdateInfo[AssemblyVersionType.AssemblyInformationalVersion];

        /// <summary>
        ///     Gets proposed versions.
        /// </summary>
        public AssemblyVersions ToBecomeAssemblyVersions { get; private set; }

        /// <summary>
        ///     Flag indicationg if project is versionable. For example,
        ///     enterprise template projects do not contain any version
        ///     information.
        /// </summary>
        public bool IsVersionable => ProjectTypeInfo.IsVersionable;

        /// <summary>
        ///     Increments project level (i.e. indentation).
        /// </summary>
        public void IncrementLevel()
        {
            Level++;
            foreach (var pi in SubProjects)
            {
                pi.IncrementLevel();
                Debug.Assert(pi.Level == Level + 1);
            }
        }

        /// <summary>
        ///     Sets "to be version" according to configuration settings.
        /// </summary>
        /// <param name="versionProvider">
        ///     Version provider.
        /// </param>
        public void SetToBecomeVersion(NewVersionProvider versionProvider)
        {
            if (CurrentAssemblyVersions != AssemblyVersions.Empty)
                ToBecomeAssemblyVersions = versionProvider.ProposeNewVersions(CurrentAssemblyVersions);
        }

        /// <summary>
        ///     Mark assembly version(s) for update.
        /// </summary>
        /// <param name="versionType">
        ///     Version type to mark. May be any combination of flags, including
        ///     <c>AssemblyVersionType.All</c>.
        /// </param>
        public void MarkAssemblyVersionsForUpdate(AssemblyVersionType versionType)
        {
            if ((versionType & AssemblyVersionType.AssemblyVersion) == AssemblyVersionType.AssemblyVersion)
                if (CurrentAssemblyVersions[AssemblyVersionType.AssemblyVersion] != ProjectVersion.Empty)
                    _assemblyVersionsUpdateInfo[AssemblyVersionType.AssemblyVersion] = true;
            if ((versionType & AssemblyVersionType.AssemblyFileVersion) == AssemblyVersionType.AssemblyFileVersion)
                if (CurrentAssemblyVersions[AssemblyVersionType.AssemblyFileVersion] != ProjectVersion.Empty)
                    _assemblyVersionsUpdateInfo[AssemblyVersionType.AssemblyFileVersion] = true;

            if ((versionType & AssemblyVersionType.AssemblyInformationalVersion) != AssemblyVersionType.AssemblyInformationalVersion) return;

            if (CurrentAssemblyVersions[AssemblyVersionType.AssemblyInformationalVersion] != ProjectVersion.Empty)
                _assemblyVersionsUpdateInfo[AssemblyVersionType.AssemblyInformationalVersion] = true;
        }

        /// <summary>
        ///     Gets a flag indicating if an assembly version is marked for
        ///     update.
        /// </summary>
        /// <param name="versionType">
        ///     Version type for which flag should be provided.
        /// </param>
        /// <returns>
        ///     <c>true</c> if version type is marked for update.
        /// </returns>
        public bool IsMarkedForUpdate(AssemblyVersionType versionType)
        {
            Debug.Assert(versionType != AssemblyVersionType.All);
            return _assemblyVersionsUpdateInfo[versionType];
        }

        public bool Save(AssemblyVersionType versionTypeToSave, string newVersion)
        {
            foreach (var vs in _versionStreams)
                try
                {
                    vs.SaveVersion(versionTypeToSave, newVersion);
                }
                catch (Exception)
                {
                    MessageBox.Show(TxtCannotSaveFile + Environment.NewLine + vs.Filename, TxtSaveErrorTitle);
                    return false;
                }
            return true;
        }

        /// <summary>
        ///     Compares <c>ProjectInfo</c> name to the name provided ignoring case.
        /// </summary>
        /// <param name="projectName">
        ///     Name to compare to.
        /// </param>
        /// <returns>
        ///     A 32-bit signed integer indicating the lexical relationship between the two comparands.
        /// </returns>
        public int CompareTo(string projectName)
        {
            return string.Compare(ProjectName, projectName, StringComparison.OrdinalIgnoreCase);
        }
    }
}