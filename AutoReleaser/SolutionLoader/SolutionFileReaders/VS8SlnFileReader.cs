/*
 * Filename:    VS8SlnFileReader.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     CommandLine
 * Description: Class that parses the content of Visual Studio 2005 and newer
 *              solution (SLN) files.
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
using System.Text.RegularExpressions;
using AutoReleaser.SolutionLoader.Configuration;
using AutoReleaser.SolutionLoader.ProjectFileReaders;

namespace AutoReleaser.SolutionLoader.SolutionFileReaders
{
    public class Vs8SlnFileReader : SlnFileReader
    {
        private const string StartOfNestedProjectsPattern = "GlobalSection\\(NestedProjects\\)\\s*=\\s*preSolution\\s*$";
        private const string EndOfNestedProjectsPattern = "\\s*EndGlobalSection";
        private const string GuidNestingPattern = GuidPattern + "\\s*=\\s*" + GuidPattern;
        private const string NestedProjectsPattern = StartOfNestedProjectsPattern + "(\\s*" + GuidNestingPattern + "\\s*$)*" + EndOfNestedProjectsPattern;


        public Vs8SlnFileReader(string solutionFilename, VcbConfiguration configuration) : base(solutionFilename, configuration)
        {
        }

        /// <summary>
        ///     Extract projects from SLN file content.
        /// </summary>
        protected override void ExtractProjects(string fileContent)
        {
            var regex = new Regex(ProjectPattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            var projects = new Dictionary<string, ProjectInfo>();
            // first create a collection of all projects
            foreach (Match match in regex.Matches(fileContent))
                try
                {
                    var pi = ExtractProjectInfo(match.Value, out var guid);
                    if (pi.ProjectTypeInfo != ProjectTypeInfo.SetupProject && pi.ProjectTypeInfo != ProjectTypeInfo.InstallShieldLeProject)
                        projects.Add(guid, pi);
                }
                catch (UnknownProjectTypeException exception)
                {
                    Trace.WriteLine($"Project \'{exception.ProjectName}\' is of unsupported type clsid: {exception.Clsid}");
                    Console.WriteLine(UnknownProjectType, exception.ProjectName);
                }
            // now rearange them into the tree structure
            SetProjectNestings(fileContent, projects);
            AllProjects.Clear();
            foreach (var pi in projects.Values) AllProjects.Add(pi);
        }

        private static void SetProjectNestings(string text, Dictionary<string, ProjectInfo> projects)
        {
            var regexNestedProjects = new Regex(NestedProjectsPattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            var guidNestings = regexNestedProjects.Match(text).Value;
            if (guidNestings.Length == 0)
                return;
            // go through project nestings from back forwards
            var regexMappings = new Regex(GuidNestingPattern, RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
            var regexGuid = new Regex(GuidPattern, RegexOptions.IgnoreCase);
            foreach (var guidPairs in from Match match in regexMappings.Matches(guidNestings) select match.Value into mappingLine select regexGuid.Matches(mappingLine))
            {
                Debug.Assert(guidPairs.Count == 2);
                var guid = guidPairs[0].Value;
                var guidParent = guidPairs[1].Value;
                var pi = projects[guid];
                // nest project only if it exists (setup project may not exist if configured so)
                if (pi == null) continue;

                
                if (!projects.TryGetValue(guidParent, out var piParent))
                    continue;
                piParent.SubProjects.Add(pi);
                pi.IncrementLevel();
                Debug.Assert(projects.ContainsKey(guid));
                projects.Remove(guid);
                Debug.Assert(!projects.ContainsKey(guid));
            }
        }

        /// <summary>
        ///     Creates and returns project file reader object.
        /// </summary>
        /// <param name="projectName">
        ///     Name of the project.
        /// </param>
        /// <param name="projectFilename">
        ///     Full filename of the project file.
        /// </param>
        /// <param name="projectTypeInfo">
        ///     <c>ProjectTypeInfo</c> identifying the type of the project.
        /// </param>
        /// <returns>
        ///     Returns <c>ProjectFileReader</c> object.
        /// </returns>
        protected override ProjectFileReader GetProjectFileReader(string projectName, string projectFilename, ProjectTypeInfo projectTypeInfo)
        {
            switch (projectTypeInfo.ProjectType)
            {
                case ProjectType.CSharpProject:
                case ProjectType.VbProject:
                case ProjectType.VjSharpProject:
                    return new VS8ProjectFileReader(projectName, projectFilename, projectTypeInfo, 0);
                case ProjectType.VCppProject:
                    if (Path.GetExtension(projectFilename) == ".vcxproj")
                        return new VcxProjectFileReader(projectName, projectFilename, projectTypeInfo, 0);
                    return new VCppProjectFileReader(projectName, projectFilename, projectTypeInfo, 0);
            }

            return null;
        }
    }
}