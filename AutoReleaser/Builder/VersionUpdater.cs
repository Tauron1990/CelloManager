/*
 * Filename:    Main.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     CommandLine
 * Description: Entry class for command-line utility.
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using AutoReleaser.SolutionLoader;
using AutoReleaser.SolutionLoader.Configuration;
using AutoReleaser.SolutionLoader.SolutionFileReaders;
using AutoReleaser.SolutionLoader.Versions;

namespace AutoReleaser.Builder
{
    public enum UpdateType
    {
        Major,
        Minor,
        Build
    }

    public class ProjectSortDescription
    {
        public string[] ProjectsAllowedForVersionModification { get; set; }
        public string[] ProjectsToExclude { get; set; }
        public string[] ProjectsToForce { get; set; }
    }

    /// <summary>
    ///     Main class used for command line tool.
    /// </summary>
    public static class VersionUpdater
    {
        #region Constructors

        public static void UpdateVersion(string path, UpdateType updateType, ProjectSortDescription sortDescription)
        {
            _solutionFilename = path;
            Debug.Assert(_solutionFilename != null);

            if (!File.Exists(_solutionFilename)) return;

            // load configuration
            var configuration = ConfigurationPersister.InstanceField.Configuration;
            if (!configuration.ConfigurationFileRead)
                Console.WriteLine("Warning: Failed to read configuration file - using default configuration.");
            // apply command line switches to modify configuration
            ApplyCommandLineArgsToIncrementScheme(configuration.NumberingOptions);
            // now validate version string
            string newVersion;

            switch (updateType)
            {
                case UpdateType.Major:
                    newVersion = "+1.*.*.*";
                    break;
                case UpdateType.Minor:
                    newVersion = "*.+1.*.*";
                    break;
                case UpdateType.Build:
                    newVersion = "*.*.+1.*";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(updateType), updateType, null);
            }

            if (newVersion.Length > 0 && !ProjectVersion.IsValidPattern(newVersion))
                if (MustBeValidVersionString())
                    throw new InvalidOperationException($"Invalid version: {newVersion}");

            _sfr = SlnFileReader.SlnFileReaderFactory.GetSlnFileReader(_solutionFilename, configuration);

            if (!ValidateSolutionIntegrity() || !ValidateProjectnames(sortDescription)) return;

            Debug.Assert(_sfr != null);
            _sfr.ApplyConfiguration(configuration);
            IProjectFilter projectFilter = new ProjectFilterByName(configuration.NumberingOptions.IncludeSetupProjects, configuration.DisplayOptions.ShowNonVersionableProjects,
                configuration.DisplayOptions.ShowSubProjectRoot, configuration.DisplayOptions.ShowEnterpriseTemplateProjectRoot,
                sortDescription.ProjectsAllowedForVersionModification, sortDescription.ProjectsToExclude, sortDescription.ProjectsToExclude);
            _sfr.ApplyFilter(projectFilter);

            ExecuteBatchCommand(configuration.NumberingOptions, sortDescription.ProjectsToForce, newVersion);
        }

        #endregion // Constructors

        #region Private methods

        private static void ExecuteBatchCommand(NumberingOptions numberingOptions, string[] projectsToForce, string newVersion)
        {
            var applyToTypes = AssemblyVersionType.All;
            MarkForUpdate(applyToTypes, projectsToForce);
            // apply versions to individual types
            if ((applyToTypes & AssemblyVersionType.AssemblyVersion) == AssemblyVersionType.AssemblyVersion)
                ApplyVersion(AssemblyVersionType.AssemblyVersion, numberingOptions, newVersion);
            if ((applyToTypes & AssemblyVersionType.AssemblyInformationalVersion) == AssemblyVersionType.AssemblyInformationalVersion)
                ApplyVersion(AssemblyVersionType.AssemblyInformationalVersion, numberingOptions, newVersion);
            if ((applyToTypes & AssemblyVersionType.AssemblyFileVersion) == AssemblyVersionType.AssemblyFileVersion)
                ApplyVersion(AssemblyVersionType.AssemblyFileVersion, numberingOptions, newVersion);
        }

        /// <summary>
        ///     Marks projects for update according to configuration settings
        ///     and command line arguments.
        /// </summary>
        /// <param name="applyToTypes">
        ///     <c>AssemblyVersionType</c> flags indicating which version types
        ///     may be updated.
        /// </param>
        /// <param name="projectsToForce"></param>
        private static void MarkForUpdate(AssemblyVersionType applyToTypes, string[] projectsToForce)
        {
            foreach (var pi in from pi in _sfr.ProjectInfoList from projectName in projectsToForce where pi.CompareTo(projectName) == 0 select pi)
                pi.MarkAssemblyVersionsForUpdate(applyToTypes);
        }


        private static void ApplyVersion(AssemblyVersionType versionToChange, NumberingOptions numberingOptions, string newVersionParm)
        {
            Debug.Assert(_sfr != null);
            Debug.Assert(versionToChange != AssemblyVersionType.All && versionToChange != AssemblyVersionType.None);
            var nvp = new NewVersionProvider(numberingOptions);
            foreach (var pi in _sfr.ProjectsToUpdate.Where(pi => pi.CurrentAssemblyVersions.ContainsVersion(versionToChange)))
                if (newVersionParm.Length == 0)
                {
                    if (!pi.ToUpdate) continue;

                    pi.SetToBecomeVersion(nvp);
                    var newVersion = pi.ToBecomeAssemblyVersions.HighestProjectVersion.ToString();
                    SaveVersion(pi, versionToChange, newVersion);
                }
                else
                {
                    if (ProjectVersion.IsValidPattern(newVersionParm))
                    {
                        var buildAndRevisionResetValue = (int) numberingOptions.ResetBuildAndRevisionTo;
                        var newVersion = ProjectVersion.ApplyVersionPattern(newVersionParm, pi.CurrentAssemblyVersions[versionToChange].ToString(), buildAndRevisionResetValue);
                        SaveVersion(pi, versionToChange, newVersion);
                    }
                    else
                    {
                        // irregular pattern cannot be applied to ProductVersion of VC++ projects
                        if (pi.ProjectTypeInfo.ProjectType == ProjectType.VCppProject) continue;

                        Debug.Assert(versionToChange == AssemblyVersionType.AssemblyInformationalVersion);
                        SaveVersion(pi, versionToChange, newVersionParm);
                    }
                }
        }

        /// <summary>
        ///     Checks if validation of version string may be performed.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if version string must be a valid one.
        /// </returns>
        private static bool MustBeValidVersionString()
        {
            return true;
        }

        /// <summary>
        ///     Saves version(s).
        /// </summary>
        /// <param name="pi">
        ///     <c>ProjectInfo</c> for which new version must be saved.
        /// </param>
        /// <param name="versionToChange">
        ///     <c>AssemblyVersionType</c> flag for version to change.
        /// </param>
        /// <param name="newVersion">
        ///     New version to apply.
        /// </param>
        private static void SaveVersion(ProjectInfo pi, AssemblyVersionType versionToChange, string newVersion)
        {
            if (pi.Save(versionToChange, newVersion))
                _sfr.UpdateSummary.SetUpdated(pi, versionToChange, newVersion);
        }

        /// <summary>
        ///     Apply command line arguments to configuration.
        /// </summary>
        /// <param name="numberingOptions">
        ///     <c>NumberingOptions</c> object.
        /// </param>
        private static void ApplyCommandLineArgsToIncrementScheme(NumberingOptions numberingOptions)
        {
            numberingOptions.BatchCommandIncrementScheme = BatchCommandIncrementScheme.IncrementAllAndSynchronize;
        }

        private const string ProjectNotFound = "Project Not Found: {0}";

        /// <summary>
        ///     Validates solution integrity i.e. if all project files and all
        ///     items in each project file are found.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if solution is complete.
        /// </returns>
        private static bool ValidateSolutionIntegrity()
        {
            using (var sw = new StringWriter())
            {
                foreach (var projectName in _sfr.ProjectsNotFound)
                    sw.WriteLine(ProjectNotFound, projectName);

                foreach (var pair in _sfr.FilesNotFound)
                {
                    var projectName = pair.Key;
                    sw.WriteLine("Files for Project Not Found: {0}", projectName);
                    var filesNotFound = pair.Value;
                    foreach (var filename in filesNotFound)
                        sw.WriteLine("  {0}", filename);
                }

                sw.Flush();
                var output = sw.ToString();
                return output.Length == 0;
            }
        }

        /// <summary>
        ///     Validates if project names provided by some switches exist in the
        ///     solution.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if all filenames provided are correct or a switch to
        ///     ignore invalid names is set.
        /// </returns>
        private static bool ValidateProjectnames(ProjectSortDescription desc)
        {
            using (var sw = new StringWriter())
            {
                foreach (var projectName in desc.ProjectsAllowedForVersionModification.Where(projectName => !_sfr.ProjectInfoList.Contains(projectName)))
                    sw.WriteLine(ProjectNotFound, projectName);
                foreach (var projectName in desc.ProjectsToExclude.Where(projectName => !_sfr.ProjectInfoList.Contains(projectName)))
                    sw.WriteLine(ProjectNotFound, projectName);
                foreach (var projectName in desc.ProjectsToForce.Where(projectName => !_sfr.ProjectInfoList.Contains(projectName)))
                    sw.WriteLine(ProjectNotFound, projectName);
                sw.Flush();
                var output = sw.ToString();
                if (output.Length > 0)
                    Console.WriteLine(sw.ToString());
                return output.Length == 0;
            }
        }

        #endregion // Private methods

        #region Private fields

        private static SolutionFileReader _sfr;


        private static string _solutionFilename;

        #endregion // Private fields
    }
}