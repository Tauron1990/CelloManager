/*
 * Filename:    VCppProjectFileReader.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     CommandLine
 * Description: Class that parses content of VC++ project (VCPROJ) file. 
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

using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using AutoReleaser.SolutionLoader.Util;

namespace AutoReleaser.SolutionLoader.ProjectFileReaders
{
    /// <summary>
    ///     Class that parses content of VC++ project (VCPROJ) file.
    /// </summary>
    public class VCppProjectFileReader : ProjectFileReader
    {
        private const string FilesTag = "Files";
        private const string FilterTag = "Filter";
        private const string SourceControlFilesAttribute = "SourceControlFiles";
        private const string RelativePathTag = "RelativePath";

        public VCppProjectFileReader(string projectName, string projectFilename, ProjectTypeInfo projectTypeInfo, int level) : base(projectName, projectFilename, projectTypeInfo, level)
        {
        }

        protected override string[] GetIncludedFiles()
        {
            var files = new ArrayList();
            var doc = new XmlDocument();
            doc.Load(ProjectFilename);
            // find 'Files' element; there should be a single one
            Debug.Assert(doc.GetElementsByTagName(FilesTag).Count == 1);
            var filesElement = (XmlElement) doc.GetElementsByTagName(FilesTag)[0];
            // find 'Filter' elements inside it
            var filterNodes = filesElement.GetElementsByTagName(FilterTag);
            foreach (var filterElement in from XmlNode filterNode in filterNodes select filterNode as XmlElement)
            {
                Debug.Assert(filterElement != null);
                var sourceControlAttribute = filterElement.GetAttribute(SourceControlFilesAttribute);
                if (sourceControlAttribute == "false") continue;

                var fileNodes = filterElement.GetElementsByTagName(FileTag);
                foreach (XmlNode fileNode in fileNodes)
                {
                    Debug.Assert(fileNode is XmlElement);
                    if (fileNode.Attributes == null) continue;

                    var attr = fileNode.Attributes[RelativePathTag];
                    var filename = FileUtil.CombinePaths(Path.GetDirectoryName(ProjectFilename), attr.Value);
                    if (File.Exists(filename))
                        files.Add(filename);
                    else
                        MissingFiles.Add(filename);
                }
            }
            return (string[]) files.ToArray(typeof(string));
        }
    }
}