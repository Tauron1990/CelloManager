/*
 * Filename:    UpdateSummary.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Stores information about version change of projects.
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
using System.IO;
using System.Linq;
using AutoReleaser.SolutionLoader.Configuration;
using AutoReleaser.SolutionLoader.Versions;

namespace AutoReleaser.SolutionLoader.Util
{
    /// <summary>
    ///     Collection containing summary of updates.
    /// </summary>
    public class UpdateSummary
    {
        public enum UpdateState
        {
            Updated,
            NotUpdated
        }

        #region UpdateSummaryItem

        public class UpdateSummaryItem
        {
            public class AssemblyVersionItem
            {
                public UpdateState UpdateState;

                public string Version;

                public AssemblyVersionItem(string version, UpdateState updateState)
                {
                    Version = version;
                    UpdateState = updateState;
                }
            }

            private readonly Dictionary<AssemblyVersionType, AssemblyVersionItem> _assemblyVersions = new Dictionary<AssemblyVersionType, AssemblyVersionItem>(AssemblyVersions.AssemblyVersionTypes.Length);

            public readonly string ProjectFullName;

            public readonly string ProjectName;

            public UpdateState UpdateState;

            public UpdateSummaryItem(ProjectInfo projectInfo, UpdateState updateState)
            {
                ProjectName = projectInfo.ProjectName;
                ProjectFullName = projectInfo.FullName;
                UpdateState = updateState;
                foreach (var versionType in AssemblyVersions.AssemblyVersionTypes)
                {
                    string version;
                    AssemblyVersionItem avi;
                    if (projectInfo.IsMarkedForUpdate(versionType) && updateState == UpdateState.Updated)
                    {
                        version = projectInfo.ToBecomeAssemblyVersions[versionType].ToString();
                        avi = new AssemblyVersionItem(version, UpdateState.Updated);
                    }
                    else
                    {
                        version = projectInfo.CurrentAssemblyVersions[versionType].ToString();
                        avi = new AssemblyVersionItem(version, UpdateState.NotUpdated);
                    }
                    _assemblyVersions.Add(versionType, avi);
                }
            }

            public AssemblyVersionItem this[AssemblyVersionType assemblyVersionType] => _assemblyVersions[assemblyVersionType];
        }

        #endregion // UpdateSummaryItem

        private const string NothingToUpdate = "No project found for version update. Please run the GUI tool.";
        private const string UpdateCaption = "VERSION UPDATE SUMMARY:";
        private const string ProjectNameCaption = "Project Name: {0}";
        private const string FullPathCaption = "Full Path:    {1}";
        private const string AssemblyVersionCaption = "  Assembly version: {0}";
        private const string ProductVersionCaption = "  Product version:  {0}";
        private const string FileVersionCaption = "  File version:     {0}";

        private readonly List<UpdateSummaryItem> _projects = new List<UpdateSummaryItem>();

        private UpdateSummaryItem this[ProjectInfo projectInfo]
        {
            get
            {
                foreach (var usi in _projects.Where(usi => usi.ProjectFullName == projectInfo.FullName))
                    return usi;
                throw new IndexOutOfRangeException($"Invalid index: {projectInfo.ProjectName} : {projectInfo.FullName}");
            }
        }

        private int UpdatedItemsCount => _projects.Count(usi => usi.UpdateState == UpdateState.Updated);

        public void AddRange(ProjectInfo[] projectInfos)
        {
            foreach (var usi in from projectInfo in projectInfos where projectInfo.IsVersionable select new UpdateSummaryItem(projectInfo, UpdateState.NotUpdated))
                _projects.Add(usi);
        }

        public void Clear()
        {
            _projects.Clear();
        }

        public void SetUpdated(ProjectInfo projectInfo, AssemblyVersionType versionType, string newVersion)
        {
            Debug.Assert(Contains(projectInfo));
            this[projectInfo].UpdateState = UpdateState.Updated;
            this[projectInfo][versionType].UpdateState = UpdateState.Updated;
            this[projectInfo][versionType].Version = newVersion;
        }

        private bool Contains(ProjectInfo projectInfo)
        {
            foreach (var usi in _projects)
                if (usi.ProjectFullName == projectInfo.FullName)
                    return true;
            return false;
        }

        public override string ToString()
        {
            using (var sw = new StringWriter())
            {
                if (UpdatedItemsCount == 0)
                {
                    sw.WriteLine(NothingToUpdate);
                }
                else
                {
                    sw.WriteLine(UpdateCaption);
                    foreach (var item in _projects.Where(item => item.UpdateState == UpdateState.Updated))
                    {
                        sw.WriteLine(ProjectNameCaption + sw.NewLine + FullPathCaption, item.ProjectName, item.ProjectFullName);
                        if (item[AssemblyVersionType.AssemblyVersion].UpdateState == UpdateState.Updated)
                            sw.WriteLine(AssemblyVersionCaption, item[AssemblyVersionType.AssemblyVersion].Version);
                        if (item[AssemblyVersionType.AssemblyInformationalVersion].UpdateState == UpdateState.Updated)
                            sw.WriteLine(ProductVersionCaption, item[AssemblyVersionType.AssemblyInformationalVersion].Version);
                        if (item[AssemblyVersionType.AssemblyFileVersion].UpdateState == UpdateState.Updated)
                            sw.WriteLine(FileVersionCaption, item[AssemblyVersionType.AssemblyFileVersion].Version);
                    }
                }
                sw.Flush();
                return sw.ToString();
            }
        }
    }
}