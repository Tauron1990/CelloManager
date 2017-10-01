/*
 * Filename:    VS8ProjectFileReader.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     CommandLine
 * Description: Classes that parse content of MSBuild2003 project 
 *              files (Visual Studio 2005 and newer .NET projects and
 *              Visual Studio 2010 and newer CPP projects.
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
using System.Text;
using System.Xml;
using System.Xml.XPath;
using AutoReleaser.SolutionLoader.Util;

namespace AutoReleaser.SolutionLoader.ProjectFileReaders
{
    /// <summary>
    ///     Abstract class used to read MSBuild2003 based project files.
    /// </summary>
    public abstract class MSBuild2003ProjectReader : ProjectFileReader
    {
        #region Constructor

        /// <summary>
        ///     Creates <code>MSBuild2003ProjectReader</code>.
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
        protected MSBuild2003ProjectReader(string projectName, string projectFilename, ProjectTypeInfo projectTypeInfo, int level)
            : base(projectName, projectFilename, projectTypeInfo, level)
        {
        }

        #endregion // Constructor

        #region ProjectFileReader overrides

        /// <summary>
        ///     Gets array with names of all included files.
        /// </summary>
        /// <returns>
        ///     An array of strings.
        /// </returns>
        protected override string[] GetIncludedFiles()
        {
            Debug.Assert(ProjectFilename != null && File.Exists(ProjectFilename));
            var files = new ArrayList();
            foreach (var nodes in ItemXPaths.Select(PrefixXPath).Select(prefixedXPath => XPathNavigator.Select(prefixedXPath, XmlNamespaceManager)))
                while (nodes.MoveNext())
                {
                    var fileName = nodes.Current.GetAttribute("Include", string.Empty);
                    var fullFilename = FileUtil.CombinePaths(Path.GetDirectoryName(ProjectFilename), fileName);
                    if (File.Exists(fullFilename))
                        files.Add(fullFilename);
                    else
                        MissingFiles.Add(fullFilename);
                }
            return (string[]) files.ToArray(typeof(string));
        }

        #endregion ProjectFileReader overrides

        #region Static methods

        /// <summary>
        ///     Add namespace prefix to XPath.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string PrefixXPath(string path)
        {
            var sb = new StringBuilder(path);
            sb.Replace("/", "/" + NamespacePrefix + ":");
            return sb.ToString();
        }

        #endregion // Static methods

        #region Protected properties

        /// <summary>
        ///     Gets <c>XmlNamespaceManager</c> using lazy evaluation.
        /// </summary>
        private XmlNamespaceManager XmlNamespaceManager
        {
            get
            {
                if (_xmlNamespaceManager == null)
                {
                    _xmlNamespaceManager = new XmlNamespaceManager(XPathNavigator.NameTable);
                    _xmlNamespaceManager.AddNamespace(NamespacePrefix, NameSpaceUri);
                }
                Debug.Assert(_xmlNamespaceManager != null);
                return _xmlNamespaceManager;
            }
        }

        /// <summary>
        ///     Gets xPath navigator used to select project item nodes in
        ///     project file.
        /// </summary>
        private XPathNavigator XPathNavigator
        {
            get
            {
                if (_xPathNavigator == null)
                    using (TextReader tr = new StreamReader(ProjectFilename))
                    {
                        var document = new XPathDocument(tr);
                        _xPathNavigator = document.CreateNavigator();
                    }
                Debug.Assert(_xPathNavigator != null);
                return _xPathNavigator;
            }
        }

        /// <summary>
        ///     Gets XPaths used to select project items.
        /// </summary>
        protected abstract string[] ItemXPaths { get; }

        #endregion Protected properties

        #region Private fields

        private XPathNavigator _xPathNavigator;

        private XmlNamespaceManager _xmlNamespaceManager;

        #endregion // Private fields

        #region Private constants

        private const string NameSpaceUri = "http://schemas.microsoft.com/developer/msbuild/2003";
        private const string NamespacePrefix = "ms";

        #endregion // Private constants
    }

    /// <summary>
    ///     Class that parses the content of Visual Studio 2005 and 2008 project
    ///     files.
    /// </summary>
    public class VS8ProjectFileReader : MSBuild2003ProjectReader
    {
        #region Constructor

        /// <summary>
        ///     Creates an instance of <code>VS8ProjectFileReader</code> class.
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
        public VS8ProjectFileReader(string projectName, string projectFilename, ProjectTypeInfo projectTypeInfo, int level)
            : base(projectName, projectFilename, projectTypeInfo, level)
        {
        }

        #endregion Constructor

        #region MSBuild2003ProjectReader overrides

        /// <summary>
        ///     Gets XPaths used to select project items.
        /// </summary>
        protected override string[] ItemXPaths => new[]
        {
            "/Project/ItemGroup/Compile",
            "/Project/ItemGroup/Include",
            "/Project/ItemGroup/EmbeddedResource"
        };

        #endregion // MSBuild2003ProjectReader overrides
    }

    /// <summary>
    ///     Class that parses the content of Visual Studio 2010 (and later)
    ///     vcxproj files.
    /// </summary>
    public class VcxProjectFileReader : MSBuild2003ProjectReader
    {
        #region Constructor

        /// <summary>
        ///     Creates an instance of <code>VcxProjectFileReader</code> class.
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
        public VcxProjectFileReader(string projectName, string projectFilename, ProjectTypeInfo projectTypeInfo, int level)
            : base(projectName, projectFilename, projectTypeInfo, level)
        {
        }

        #endregion // Constructor

        #region MSBuild2003ProjectReader overrides

        /// <summary>
        ///     Gets XPaths used to select project items.
        /// </summary>
        protected override string[] ItemXPaths => new[]
        {
            "/Project/ItemGroup/ClCompile",
            "/Project/ItemGroup/ClInclude",
            "/Project/ItemGroup/ResourceCompile",
            "/Project/ItemGroup/EmbeddedResource"
        };

        #endregion // MSBuild2003ProjectReader overrides
    }
}