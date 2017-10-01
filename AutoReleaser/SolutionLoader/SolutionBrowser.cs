/*
 * Filename:    SolutionBrowser.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Interface and base class for solution browsers 
 *              (VSSolutionBrowser and SolutionFileReader).
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
using System.Linq;
using AutoReleaser.SolutionLoader.Configuration;
using AutoReleaser.SolutionLoader.Util;
using AutoReleaser.SolutionLoader.Versions;

namespace AutoReleaser.SolutionLoader
{
    /// <summary>
    ///     Base class for all solution browsers. Implements <c>ISolutionBrowser</c>
    ///     interface.
    /// </summary>
    public abstract class SolutionBrowser
    {
        #region Private properties

        /// <summary>
        ///     Gets the mask for <c>AssemblyVersionType</c> object.
        /// </summary>
        private AssemblyVersionType AssemblyVersionsUpdateMask
        {
            get
            {
                Debug.Assert(_numberingOptions != null);
                return _numberingOptions.ApplyToAllTypes ? AssemblyVersionType.All : _numberingOptions.DefaultVersionType;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        ///     Creates an empty instance of <c>SolutionBrowser</c> object.
        /// </summary>
        private SolutionBrowser()
        {
            AllProjects = new SortedSet<ProjectInfo>(new ProjectComparer());
            _updateSummary = new UpdateSummary();
            _projectFilter = ProjectFilter.PassAll;
            Debug.Assert(AllProjects != null);
            Debug.Assert(_updateSummary != null);
        }

        /// <summary>
        ///     Creates <c>SolutionBrowser</c> object with configuration provided.
        /// </summary>
        /// <param name="configuration">
        ///     Configuration to be used.
        /// </param>
        protected SolutionBrowser(VcbConfiguration configuration) : this()
        {
            Debug.Assert(configuration?.NumberingOptions != null);
            SetNumberingOptions(configuration.NumberingOptions);
        }

        #endregion // Constructor

        #region Public properties

        /// <summary>
        ///     Gets a list of <c>ProjectInfo</c> objects currently displayed.
        /// </summary>
        public ProjectInfoList ProjectInfoList
        {
            get
            {
                if (_filteredProjects == null)
                    UpdateFilteredProjectsList();
                return _filteredProjects;
            }
        }

        /// <summary>
        ///     Gets an array of <c>ProjectInfo</c> objects for which version
        ///     should be updated.
        /// </summary>
        public ProjectInfo[] ProjectsToUpdate
        {
            get
            {
                var projectsToUpdate = new List<ProjectInfo>();
                foreach (var projectInfo in ProjectInfoList)
                    if (_newVersionProvider.ShouldUpdate(projectInfo, AssemblyVersionsUpdateMask, ProjectInfoList.HighestToBeAssemblyVersions.HighestProjectVersion)) projectsToUpdate.Add(projectInfo);
                return projectsToUpdate.ToArray();
            }
        }

        /// <summary>
        ///     Gets <c>UpdateSummary</c> with update info.
        /// </summary>
        public UpdateSummary UpdateSummary
        {
            get
            {
                Debug.Assert(_updateSummary != null);
                return _updateSummary;
            }
        }

        #endregion // Public properties

        #region Implemented public methods

        /// <summary>
        ///     Applies configuration settings to projects found.
        /// </summary>
        /// <param name="configuration">
        ///     Configuration to apply.
        /// </param>
        public void ApplyConfiguration(VcbConfiguration configuration)
        {
            Debug.Assert(AllProjects != null);
            Debug.Assert(configuration?.NumberingOptions != null);
            SetNumberingOptions(configuration.NumberingOptions);
            _filteredProjects = null;
        }

        /// <summary>
        ///     Applies a filter on project infos.
        /// </summary>
        /// <param name="filter">
        ///     Filter to apply.
        /// </param>
        public void ApplyFilter(IProjectFilter filter)
        {
            Debug.Assert(filter != null);
            _projectFilter = filter;
            _filteredProjects = null;
        }

        #endregion // Implemented public methods

        #region Abstract members

        //public abstract void CheckOutProjectVersionFiles(ProjectInfo[] projectsToCheckOut);

        #endregion // Abstract members

        #region Private methods

        /// <summary>
        ///     Recurses projects tree built to linearize it.
        /// </summary>
        /// <param name="projects">
        /// </param>
        /// <returns>
        /// </returns>
        private ICollection<ProjectInfo> RecurseSubProjects(ICollection<ProjectInfo> projects)
        {
            var subProjects = new List<ProjectInfo>();
            foreach (var pi in projects)
            {
                var rs = RecurseSubProjects(pi.SubProjects);
                //if (ShouldDisplay(pi.ProjectTypeInfo.ProjectType, rs.Count)) {
                subProjects.Add(pi);
                subProjects.AddRange(rs);
                //}
            }
            return subProjects;
        }

        ///// <summary>
        /////   Checks if some item should be displayed.
        ///// </summary>
        ///// <param name="projectType"></param>
        ///// <param name="subProjectsCount"></param>
        ///// <returns></returns>
        //private bool ShouldDisplay(ProjectType projectType, int subProjectsCount) {
        //    if ((projectType != ProjectType.VirtualFolder) && (projectType != ProjectType.SolutionFolder))
        //        return true;
        //    if (subProjectsCount > 0 || ConfigurationPersister.InstanceField.Configuration.DisplayOptions.ShowEmptyFolders) {
        //        return true;
        //    }
        //    return false;
        //}

        /// <summary>
        ///     Updates the list of filtered projects, optionally marking "forced"
        ///     projects for update.
        /// </summary>
        private void UpdateFilteredProjectsList()
        {
            Debug.Assert(_newVersionProvider != null);
            var projects = new List<ProjectInfo>(RecurseSubProjects(AllProjects));
            var projectInfos = projects.ToArray();
            // check which projects are in the list of "forced" projects
            foreach (var projectInfo in projectInfos.Where(projectInfo => Array.IndexOf(_projectFilter.ProjectsToForce, projectInfo.ProjectName) != -1))
                projectInfo.MarkAssemblyVersionsForUpdate(AssemblyVersionType.All);
            _filteredProjects = new ProjectInfoList(projectInfos, _projectFilter, _newVersionProvider, AssemblyVersionsUpdateMask);
            _updateSummary.Clear();
            _updateSummary.AddRange(_filteredProjects.ProjectInfos);
        }

        private void SetNumberingOptions(NumberingOptions options)
        {
            Debug.Assert(options != null);
            _numberingOptions = options;
            _newVersionProvider = new NewVersionProvider(_numberingOptions);
        }

        #endregion // Private methods

        #region Protected fields

        /// <summary>
        ///     Collection of <c>ProjectInfo</c> objects for the current solution.
        /// </summary>
        protected readonly SortedSet<ProjectInfo> AllProjects;

        /// <summary>
        ///     Collection of <c>ProjectInfo</c> objects currently displayed.
        /// </summary>
        private ProjectInfoList _filteredProjects;

        ///// <summary>
        /////   Object responsible to checkout items under source control.
        ///// </summary>
        //protected ISourceSafeCheckout       m_sourceSafeCheckOut;
        /// <summary>
        ///     Object responsible to provide a new version.
        /// </summary>
        private NewVersionProvider _newVersionProvider;

        /// <summary>
        ///     Configuration used.
        /// </summary>
        private NumberingOptions _numberingOptions;

        private IProjectFilter _projectFilter;

        /// <summary>
        ///     Collection with update information.
        /// </summary>
        private readonly UpdateSummary _updateSummary;

        #endregion // Protected fields
    }
}