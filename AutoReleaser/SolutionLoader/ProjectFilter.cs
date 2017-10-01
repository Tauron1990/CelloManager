/*
 * Filename:    ProjectFilter.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Interface and some implementations of class used to filter 
 *              ProjectInfo objects.
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

namespace AutoReleaser.SolutionLoader
{
    public interface IProjectFilter
    {
        string[] ProjectsToForce { get; }
        bool Pass(ProjectInfo projectInfo);
    }

    /// <summary>
    ///     Project filter passing all <c>ProjectInfo</c> objects.
    /// </summary>
    public class ProjectFilter : IProjectFilter
    {
        public static readonly ProjectFilter PassAll = new ProjectFilter();

        protected ProjectFilter()
        {
        }

        #region IProjectFilter Members

        public virtual bool Pass(ProjectInfo projectInfo)
        {
            return true;
        }

        public virtual string[] ProjectsToForce => new string[0];

        #endregion
    }

    /// <summary>
    ///     <c>IProjectFilter</c> implementation that passes <c>ProjectInfo</c>
    ///     objects using configuration settings.
    /// </summary>
    public class ProjectFilterByType : ProjectFilter
    {
        private readonly bool _passEnterpriseProjectRoots;

        private readonly bool _passNonVersionableProjects;

        private readonly bool _passProjectFolders;

        private readonly bool _passSetupProjects;

        protected ProjectFilterByType(bool passSetupProjects, bool passNonVersionableProjects, bool passProjectFolders, bool passEnterpriseProjectRoots)
        {
            _passSetupProjects = passSetupProjects;
            _passNonVersionableProjects = passNonVersionableProjects;
            _passProjectFolders = passProjectFolders;
            _passEnterpriseProjectRoots = passEnterpriseProjectRoots;
        }

        /// <summary>
        ///     Filter method.
        /// </summary>
        /// <param name="projectInfo">
        ///     <c>ProjectInfo</c> object on which filter is applied.
        /// </param>
        /// <returns>
        ///     Return <c>true</c> if project passed filter criteria.
        /// </returns>
        public override bool Pass(ProjectInfo projectInfo)
        {
            var pt = projectInfo.ProjectTypeInfo.ProjectType;
            switch (pt)
            {
                case ProjectType.SetupProject:
                    return _passSetupProjects;
                case ProjectType.FileBasedWebProject:
                case ProjectType.DatabaseProject:
                case ProjectType.FSharpProject:
                    return _passNonVersionableProjects;
                case ProjectType.SolutionFolder:
                case ProjectType.VirtualFolder:
                    return _passProjectFolders;
                case ProjectType.EnterpriseProject:
                    return _passEnterpriseProjectRoots;
            }
            return true;
        }
    }

    /// <summary>
    ///     Object used by command-line utility to filter projects by their
    ///     name.
    /// </summary>
    public class ProjectFilterByName : ProjectFilterByType
    {
        private readonly string[] _projectsToExclude;
        private readonly string[] _projectsToForce;

        private readonly string[] _projectsToInclude;

        public ProjectFilterByName(bool passSetupProjects, bool passNonVersionableProjects, bool passProjectFolders, bool passEnterpriseProjectRoots, string[] projectsToInclude, string[] projectsToExclude, string[] projectsToForce) : base(
            passSetupProjects, passNonVersionableProjects, passProjectFolders, passEnterpriseProjectRoots)
        {
            _projectsToInclude = projectsToInclude ?? new string[0];
            _projectsToExclude = projectsToExclude ?? new string[0];
            _projectsToForce = projectsToForce ?? new string[0];
        }

        public override string[] ProjectsToForce => _projectsToForce;

        public override bool Pass(ProjectInfo projectInfo)
        {
            var enumerator = _projectsToForce.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var entry = (string) enumerator.Current;
                if (ProjectNamesAreEqual(entry, projectInfo))
                    return true;
            }
            // if any of lists is not empty
            if (_projectsToExclude.Length <= 0 && _projectsToInclude.Length <= 0) return base.Pass(projectInfo);

            // first the exclusion list
            enumerator = _projectsToExclude.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var entry = (string) enumerator.Current;
                if (ProjectNamesAreEqual(entry, projectInfo))
                    return false;
            }
            // if there is an inclusion list
            if (_projectsToInclude.Length <= 0) return base.Pass(projectInfo);

            enumerator = _projectsToInclude.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var entry = (string) enumerator.Current;
                if (ProjectNamesAreEqual(entry, projectInfo))
                    return true;
            }
            return false;
        }


        private bool ProjectNamesAreEqual(string name, ProjectInfo projectInfo)
        {
            return string.Compare(name, projectInfo.ProjectName, StringComparison.OrdinalIgnoreCase) == 0;
        }
    }
}