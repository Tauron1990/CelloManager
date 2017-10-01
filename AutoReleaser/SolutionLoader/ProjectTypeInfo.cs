/*
 * Filename:    ProjectTypeInfo.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: All supported project types with relevant information.
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

namespace AutoReleaser.SolutionLoader
{
    /// <summary>
    ///     Enumeration containing all possible types in a project.
    /// </summary>
    public enum ProjectType
    {
        CSharpProject,
        SdecSharpProject,
        VbProject,
        SdevbProject,
        VjSharpProject,
        VCppProject,
        SetupProject,
        EnterpriseProject,
        SolutionFolder,
        FileBasedWebProject,
        DatabaseProject,
        FSharpProject,
        InstallShieldLeProject,
        VirtualFolder,
        SqlDatabaseProject
    }

    /// <summary>
    ///     A table of all supported project types with relevant information
    ///     like AssemblyInfo filename and icon index.
    /// </summary>
    public class ProjectTypeInfo : ICloneable
    {
        #region ProjectTypeInfoCollection class

        /// <summary>
        ///     Lookup table of all possible project types identified by their CLSID.
        /// </summary>
        public sealed class ProjectTypeInfoCollection
        {
            private readonly Dictionary<string, ProjectTypeInfo> _projectTypes;

            public ProjectTypeInfoCollection()
            {
                _projectTypes = new Dictionary<string, ProjectTypeInfo>
                {
                    {"{fae04ec0-301f-11d3-bf4b-00c04f79efbc}", CSharpProject},
                    {"{20d4826a-c6fa-45db-90f4-c717570b9f32}", SdeccSharpProject},
                    {"{f184b08f-c81c-45f6-a57f-5abd9991f28f}", VbProject},
                    {"{cb4ce8c6-1bdb-4dc7-a4d3-65a1999772f8}", SdevbProject},
                    {"{e6fdf86b-f3d1-11d4-8576-0002a516ece8}", VjSharpProject},
                    {"{8bc9ceb8-8b4a-11d0-8d11-00a0c91bc942}", VCppProject},
                    {"{54435603-dbb4-11d2-8724-00a0c9a8b90c}", SetupProject},
                    {"{7d353b21-6e36-11d2-b35a-0000f81f0c06}", EnterpriseProject},
                    {"{fe3bbbb6-72d5-11d2-9ace-00c04f79a2a4}", EnterpriseProject},
                    {"{66a26720-8fb5-11d2-aa7e-00c04f688dde}", SolutionFolder},
                    {"{2150e333-8fdc-42a3-9474-1a3956d46de8}", SolutionFolder},
                    {"{e24c65dc-7377-472b-9aba-bc803b73c61a}", FileBasedWebProject},
                    {"{c8d11400-126e-41cd-887f-60bd40844f9e}", SqlDatabaseProject},
                    {"{4f174c21-8c12-11d0-8340-0000f80270f8}", DatabaseProject},
                    {"{6bb5f8f0-4483-11d3-8bcf-00c04f8ec28c}", VirtualFolder},
                    {"{f2a71f9b-5d33-465a-a702-920d77279786}", FSharpProject},
                    {"{fbb4bd86-bf63-432a-a6fb-6cf3a1288f83}", InstallShieldLeProject},
                    {"{6141683f-8a12-4e36-9623-2eb02b2c2303}", InstallShieldLeProject}
                };
                // DTE version
                // file version
                // DTE version
                // file version
                // DTE version
                // file version
                // DTE version
                // DTE version
                // file version
            }

            public ProjectTypeInfo this[string projectType] => _projectTypes[projectType.ToLower()];
        }

        #endregion // ProjectTypeInfoCollection class

        #region Constructors

        /// <summary>
        ///     Initializes <c>ProjectTypeInfo</c> object.
        /// </summary>
        /// <param name="projectType">
        ///     Project type.
        /// </param>
        /// <param name="assemblyInfoFilename">
        ///     Name of the assembly info file.
        /// </param>
        private ProjectTypeInfo(ProjectType projectType, string assemblyInfoFilename)
        {
            ProjectType = projectType;
            AssemblyInfoFilename = assemblyInfoFilename;
            IconIndex = GetIconIndex(projectType);
        }

        #endregion // Constructors

        #region Public properties

        /// <summary>
        ///     Gets a flag if project is versionable.
        /// </summary>
        public bool IsVersionable
        {
            get
            {
                switch (ProjectType)
                {
                    case ProjectType.CSharpProject:
                    case ProjectType.SdecSharpProject:
                    case ProjectType.VbProject:
                    case ProjectType.SdevbProject:
                    case ProjectType.VjSharpProject:
                    case ProjectType.VCppProject:
                    case ProjectType.SetupProject:
                    case ProjectType.InstallShieldLeProject:
                        return true;
                    case ProjectType.EnterpriseProject:
                    case ProjectType.SolutionFolder:
                    case ProjectType.FileBasedWebProject:
                    case ProjectType.DatabaseProject:
                    case ProjectType.VirtualFolder:
                    case ProjectType.FSharpProject:
                    case ProjectType.SqlDatabaseProject:
                        return false;
                }
                Debug.Assert(false, $"Not supported ProjectType: {ProjectType.ToString()}");
                return false;
            }
        }

        #endregion // Public properties

        #region Public methods

        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion // Public methods

        #region Private methods

        /// <summary>
        ///     Gets icon index.
        /// </summary>
        /// <param name="projectType"></param>
        /// <returns></returns>
        private int GetIconIndex(ProjectType projectType)
        {
            switch (projectType)
            {
                case ProjectType.CSharpProject:
                case ProjectType.SdecSharpProject:
                case ProjectType.VbProject:
                case ProjectType.SdevbProject:
                case ProjectType.VjSharpProject:
                case ProjectType.VCppProject:
                case ProjectType.SetupProject:
                case ProjectType.EnterpriseProject:
                case ProjectType.SolutionFolder:
                case ProjectType.FileBasedWebProject:
                case ProjectType.DatabaseProject:
                case ProjectType.FSharpProject:
                case ProjectType.InstallShieldLeProject:
                    return (int) projectType * 2;
                case ProjectType.VirtualFolder:
                    return (int) ProjectType.SolutionFolder * 2;
                case ProjectType.SqlDatabaseProject:
                    return (int) ProjectType.DatabaseProject * 2;
            }
            Debug.Assert(false, $"Not supported ProjectType: {projectType.ToString()}");
            return 0;
        }

        #endregion // Private methods

        #region Public readonly fields

        /// <summary>
        ///     Gets <c>ProjectType</c>.
        /// </summary>
        public readonly ProjectType ProjectType;

        /// <summary>
        ///     Gets assembly info filename.
        /// </summary>
        public readonly string AssemblyInfoFilename;

        /// <summary>
        ///     Gets icon index used in listview.
        /// </summary>
        public readonly int IconIndex;

        #endregion // Public readonly fields

        #region Static fields

        private static readonly ProjectTypeInfo CSharpProject = new ProjectTypeInfo(ProjectType.CSharpProject, "AssemblyInfo.cs");
        private static readonly ProjectTypeInfo SdeccSharpProject = new ProjectTypeInfo(ProjectType.SdecSharpProject, "AssemblyInfo.cs");
        private static readonly ProjectTypeInfo VbProject = new ProjectTypeInfo(ProjectType.VbProject, "AssemblyInfo.vb");
        private static readonly ProjectTypeInfo SdevbProject = new ProjectTypeInfo(ProjectType.SdevbProject, "AssemblyInfo.vb");
        private static readonly ProjectTypeInfo VjSharpProject = new ProjectTypeInfo(ProjectType.VjSharpProject, "AssemblyInfo.jsl");
        private static readonly ProjectTypeInfo VCppProject = new ProjectTypeInfo(ProjectType.VCppProject, "AssemblyInfo.cpp");
        public static readonly ProjectTypeInfo SetupProject = new ProjectTypeInfo(ProjectType.SetupProject, "*.vdproj");
        private static readonly ProjectTypeInfo EnterpriseProject = new ProjectTypeInfo(ProjectType.EnterpriseProject, "");
        private static readonly ProjectTypeInfo SolutionFolder = new ProjectTypeInfo(ProjectType.SolutionFolder, "");
        public static readonly ProjectTypeInfo FileBasedWebProject = new ProjectTypeInfo(ProjectType.FileBasedWebProject, "");
        private static readonly ProjectTypeInfo DatabaseProject = new ProjectTypeInfo(ProjectType.DatabaseProject, "");
        private static readonly ProjectTypeInfo VirtualFolder = new ProjectTypeInfo(ProjectType.VirtualFolder, "");
        private static readonly ProjectTypeInfo FSharpProject = new ProjectTypeInfo(ProjectType.FSharpProject, "");
        public static readonly ProjectTypeInfo InstallShieldLeProject = new ProjectTypeInfo(ProjectType.InstallShieldLeProject, "*.isl");
        private static readonly ProjectTypeInfo SqlDatabaseProject = new ProjectTypeInfo(ProjectType.SqlDatabaseProject, "");

        public static readonly ProjectTypeInfoCollection ProjectTypeInfos = new ProjectTypeInfoCollection();

        #endregion // Static fields
    }
}