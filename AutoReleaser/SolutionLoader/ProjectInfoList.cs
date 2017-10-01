/*
 * Filename:    ProjectInfoList.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: List of ProjectInfo objects.
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
using System.Linq;
using AutoReleaser.SolutionLoader.Configuration;
using AutoReleaser.SolutionLoader.Versions;

namespace AutoReleaser.SolutionLoader
{
    /// <summary>
    ///     A list of all projects satisfying the filter provided.
    /// </summary>
    public class ProjectInfoList : IList<ProjectInfo>
    {
        private readonly List<ProjectInfo> _projectInfos;

        #region Constructors

        public ProjectInfoList(ProjectInfo[] projectInfos, IProjectFilter filter, NewVersionProvider newVersionProvider, AssemblyVersionType assemblyVersionsUpdateMask)
        {
            _projectInfos = new List<ProjectInfo>(projectInfos.Length);
            HighestToBeAssemblyVersions = AssemblyVersions.MinValue;
            HighestProposedAssemblyVersions = AssemblyVersions.MinValue;
            foreach (var pi in projectInfos.Where(filter.Pass))
            {
                pi.SetToBecomeVersion(newVersionProvider);
                if (pi.Modified)
                    pi.MarkAssemblyVersionsForUpdate(assemblyVersionsUpdateMask);
                _projectInfos.Add(pi);
                HighestToBeAssemblyVersions = AssemblyVersions.Max(HighestToBeAssemblyVersions, pi);
                HighestProposedAssemblyVersions = AssemblyVersions.MaxProposed(HighestProposedAssemblyVersions, pi);
            }
        }

        #endregion // Constructors

        public IEnumerator<ProjectInfo> GetEnumerator()
        {
            return _projectInfos.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(ProjectInfo item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(ProjectInfo item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(ProjectInfo[] array, int arrayIndex)
        {
            _projectInfos.CopyTo(array, arrayIndex);
        }

        public bool Remove(ProjectInfo item)
        {
            throw new NotImplementedException();
        }

        public int Count => _projectInfos.Count;
        public bool IsReadOnly => true;

        public int IndexOf(ProjectInfo item)
        {
            return _projectInfos.IndexOf(item);
        }

        public void Insert(int index, ProjectInfo item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public ProjectInfo this[int index]
        {
            get => _projectInfos[index];
            set => throw new NotImplementedException();
        }

        #region Public methods

        /// <summary>
        ///     Checks if project with given name exists in the list of
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public bool Contains(string projectName)
        {
            return _projectInfos.Any(projectInfo => string.Compare(projectInfo.ProjectName, projectName, StringComparison.OrdinalIgnoreCase) == 0);
        }

        #endregion // Public methods

        #region Public properties

        /// <summary>
        ///     Gets a list of filtered <c>ProjectInfo</c> objects.
        /// </summary>
        public ProjectInfo[] ProjectInfos => _projectInfos.ToArray();

        /// <summary>
        ///     Gets the highest of all "to be" project versions in the current solution.
        /// </summary>
        public AssemblyVersions HighestToBeAssemblyVersions { get; }

        /// <summary>
        ///     Gets the highest version among those marked for update.
        /// </summary>
        public AssemblyVersions HighestProposedAssemblyVersions { get; }

        #endregion //Public properties
    }
}