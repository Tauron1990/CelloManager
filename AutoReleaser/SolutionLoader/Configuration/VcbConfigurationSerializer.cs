/*
 * Filename:    VcbConfigurationSerializer.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Serializes and deserializes configuration. 
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
using System.Xml;

namespace AutoReleaser.SolutionLoader.Configuration
{
    /// <summary>
    ///     Class responsible to read and save tool configuration.
    ///     Alhough generic <c>XmlSerializer</c> class can be used as well, this
    ///     implementation avoids on-the-fly generation of serialization class by
    ///     .NET Framework and thus makes serialization/deserialization quicker.
    ///     Moreover, generic <c>XmlSerializer</c> class in some cases throws
    ///     exceptions for no obvious reason that cannot be caught.
    /// </summary>
    public class VcbConfigurationSerializer
    {
        #region Reader class

        private sealed class VcbConfigurationXmlReader : IDisposable
        {
            #region Public method

            public VcbConfiguration ReadConfiguration()
            {
                _reader.Read();
                _reader.MoveToContent();
                if (_reader.NodeType != XmlNodeType.Element) throw InvalidNodeType();

                if (_reader.LocalName == Tag.VcbConfiguration)
                    return ReadVcbConfiguration();

                throw InvalidElementName();
            }

            #endregion // Public method

            #region Constructor, finalize and dispose related methods

            public VcbConfigurationXmlReader(string filename)
            {
                _reader = new XmlTextReader(filename);
                //InitIDs();
            }

            ~VcbConfigurationXmlReader()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
                Dispose(true);
            }

            // ReSharper disable once UnusedParameter.Local
            private void Dispose(bool disposing)
            {
                if (_disposed) return;
                _reader.Close();
                _disposed = true;
            }

            #endregion // Constructor, finalize and dispose related methods

            #region Private fields

            private readonly XmlTextReader _reader;
            private bool _disposed;

            #endregion // Private fields

            #region Private method

            private XmlException InvalidElementName()
            {
                return new XmlException($"Invalid element name {_reader.LocalName} at line: {_reader.LineNumber} position: {_reader.LinePosition}");
            }

            private XmlException InvalidNodeType()
            {
                return new XmlException($"{_reader.NodeType} node {_reader.LocalName} not valid at line: {_reader.LineNumber} position: {_reader.LinePosition}");
            }

            private VcbConfiguration ReadVcbConfiguration()
            {
                var o = new VcbConfiguration();
                _reader.MoveToElement();
                if (_reader.IsEmptyElement)
                {
                    _reader.Skip();
                    return o;
                }
                var paramsRead = new bool[7];
                _reader.ReadStartElement();
                _reader.MoveToContent();
                while (_reader.NodeType != XmlNodeType.EndElement)
                {
                    if (_reader.NodeType == XmlNodeType.Element)
                        if (!paramsRead[0] && _reader.LocalName == Tag.MainFormSize)
                        {
                            _reader.Skip();
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && _reader.LocalName == Tag.ListViewColumnWidths)
                        {
                            o.ListViewColumnWidths = ReadListViewColumnWidths();
                            paramsRead[1] = true;
                        }
                        else if (!paramsRead[2] && _reader.LocalName == Tag.ApplyToAllTabsChecked)
                        {
                            o.ApplyToAllTabsChecked = bool.Parse(_reader.ReadElementString());
                            paramsRead[2] = true;
                        }
                        else if (!paramsRead[3] && _reader.LocalName == Tag.NumberingOptions)
                        {
                            o.NumberingOptions = ReadNumberingOptions();
                            paramsRead[3] = true;
                        }
                        else if (!paramsRead[4] && _reader.LocalName == Tag.DisplayOptions)
                        {
                            o.DisplayOptions = ReadDisplayOptions();
                            paramsRead[4] = true;
                        }
                        else if (!paramsRead[5] && _reader.LocalName == Tag.FoldersConfigurations)
                        {
                            o.FoldersConfigurations = ReadFoldersConfigurations();
                            paramsRead[5] = true;
                        }
                        else if (!paramsRead[6] && _reader.LocalName == Tag.ExportConfiguration)
                        {
                            o.ExportConfiguration = ReadExportConfiguration();
                            paramsRead[6] = true;
                        }
                        else
                        {
                            continue;
                        }
                    else
                        throw InvalidNodeType();
                    _reader.MoveToContent();
                }
                _reader.ReadEndElement();
                return o;
            }

            private ListViewColumnWidths ReadListViewColumnWidths()
            {
                var lvcw = new ListViewColumnWidths();
                _reader.MoveToElement();
                if (_reader.IsEmptyElement)
                {
                    _reader.Skip();
                    return lvcw;
                }
                var paramsRead = new bool[4];
                _reader.ReadStartElement();
                _reader.MoveToContent();
                while (_reader.NodeType != XmlNodeType.EndElement)
                {
                    if (_reader.NodeType == XmlNodeType.Element)
                        if (!paramsRead[0] && _reader.LocalName == Tag.ProjectName)
                        {
                            lvcw.ProjectName = int.Parse(_reader.ReadElementString());
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && _reader.LocalName == Tag.CurrentVersion)
                        {
                            lvcw.CurrentVersion = int.Parse(_reader.ReadElementString());
                            paramsRead[1] = true;
                        }
                        else if (!paramsRead[2] && _reader.LocalName == Tag.Modified)
                        {
                            lvcw.Modified = int.Parse(_reader.ReadElementString());
                            paramsRead[2] = true;
                        }
                        else if (!paramsRead[3] && _reader.LocalName == Tag.ToBeVersion)
                        {
                            lvcw.ToBeVersion = int.Parse(_reader.ReadElementString());
                            paramsRead[3] = true;
                        }
                        else
                        {
                            throw InvalidElementName();
                        }
                    else
                        throw InvalidNodeType();
                    _reader.MoveToContent();
                }
                _reader.ReadEndElement();
                return lvcw;
            }

            private NumberingOptions ReadNumberingOptions()
            {
                var no = new NumberingOptions();
                _reader.MoveToElement();
                if (_reader.IsEmptyElement)
                {
                    _reader.Skip();
                    return no;
                }
                var paramsRead = new bool[19];
                _reader.ReadStartElement();
                _reader.MoveToContent();
                while (_reader.NodeType != XmlNodeType.EndElement)
                {
                    if (_reader.NodeType == XmlNodeType.Element)
                        if (!paramsRead[0] && _reader.LocalName == Tag.SaveModifiedFilesBeforeRunningAddinCommand)
                        {
                            no.SaveModifiedFilesBeforeRunningAddinCommand = bool.Parse(_reader.ReadElementString());
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && _reader.LocalName == Tag.DefaultVersionType)
                        {
                            no.DefaultVersionType = ReadAssemblyVersionType(_reader.ReadElementString());
                            paramsRead[1] = true;
                        }
                        else if (!paramsRead[2] && _reader.LocalName == Tag.IncrementBy)
                        {
                            no.IncrementBy = int.Parse(_reader.ReadElementString());
                            paramsRead[2] = true;
                        }
                        else if (!paramsRead[3] && _reader.LocalName == Tag.AllowArbitraryInformationalVersion)
                        {
                            no.AllowArbitraryInformationalVersion = bool.Parse(_reader.ReadElementString());
                            paramsRead[3] = true;
                        }
                        else if (!paramsRead[4] && _reader.LocalName == Tag.IncludeVCppResourceFiles)
                        {
                            no.IncludeVCppResourceFiles = bool.Parse(_reader.ReadElementString());
                            paramsRead[4] = true;
                        }
                        else if (!paramsRead[5] && _reader.LocalName == Tag.IncludeSetupProjects)
                        {
                            no.IncludeSetupProjects = bool.Parse(_reader.ReadElementString());
                            paramsRead[5] = true;
                        }
                        else if (!paramsRead[6] && _reader.LocalName == Tag.GeneratePackageAndProductCodes)
                        {
                            no.GeneratePackageAndProductCodes = bool.Parse(_reader.ReadElementString());
                            paramsRead[6] = true;
                        }
                        else if (!paramsRead[7] && _reader.LocalName == Tag.ApplyToAllTypes)
                        {
                            no.ApplyToAllTypes = bool.Parse(_reader.ReadElementString());
                            paramsRead[7] = true;
                        }
                        else if (!paramsRead[8] && _reader.LocalName == Tag.SynchronizeAllVersionTypes)
                        {
                            no.SynchronizeAllVersionTypes = bool.Parse(_reader.ReadElementString());
                            paramsRead[8] = true;
                        }
                        else if (!paramsRead[9] && _reader.LocalName == Tag.IncrementScheme)
                        {
                            no.IncrementScheme = ReadIncrementScheme(_reader.ReadElementString());
                            paramsRead[9] = true;
                        }
                        else if (!paramsRead[10] && _reader.LocalName == Tag.BatchCommandIncrementScheme)
                        {
                            no.BatchCommandIncrementScheme = ReadBatchCommandIncrementScheme(_reader.ReadElementString());
                            paramsRead[10] = true;
                        }
                        else if (!paramsRead[11] && _reader.LocalName == Tag.UseDateTimeBasedBuildAndRevisionNumbering)
                        {
                            no.UseDateTimeBasedBuildAndRevisionNumbering = bool.Parse(_reader.ReadElementString());
                            paramsRead[11] = true;
                        }
                        else if (!paramsRead[12] && _reader.LocalName == Tag.ResetBuildOnMajorIncrement)
                        {
                            no.ResetBuildOnMajorIncrement = bool.Parse(_reader.ReadElementString());
                            paramsRead[12] = true;
                        }
                        else if (!paramsRead[13] && _reader.LocalName == Tag.ResetBuildOnMinorIncrement)
                        {
                            no.ResetBuildOnMinorIncrement = bool.Parse(_reader.ReadElementString());
                            paramsRead[13] = true;
                        }
                        else if (!paramsRead[14] && _reader.LocalName == Tag.ResetRevisionOnMajorIncrement)
                        {
                            no.ResetRevisionOnMajorIncrement = bool.Parse(_reader.ReadElementString());
                            paramsRead[14] = true;
                        }
                        else if (!paramsRead[15] && _reader.LocalName == Tag.ResetRevisionOnMinorIncrement)
                        {
                            no.ResetRevisionOnMinorIncrement = bool.Parse(_reader.ReadElementString());
                            paramsRead[15] = true;
                        }
                        else if (!paramsRead[16] && _reader.LocalName == Tag.ResetRevisionOnBuildIncrement)
                        {
                            no.ResetRevisionOnBuildIncrement = bool.Parse(_reader.ReadElementString());
                            paramsRead[16] = true;
                        }
                        else if (!paramsRead[17] && _reader.LocalName == Tag.ResetBuildAndRevisionTo)
                        {
                            no.ResetBuildAndRevisionTo = ReadResetBuildAndRevision(_reader.ReadElementString());
                            paramsRead[17] = true;
                        }
                        else if (!paramsRead[18] && _reader.LocalName == Tag.ReplaceAsteriskWithVersionComponents)
                        {
                            no.ReplaceAsteriskWithVersionComponents = bool.Parse(_reader.ReadElementString());
                            paramsRead[18] = true;
                        }
                        else
                        {
                            throw InvalidElementName();
                        }
                    else
                        throw InvalidNodeType();
                    _reader.MoveToContent();
                }
                _reader.ReadEndElement();
                return no;
            }

            private AssemblyVersionType ReadAssemblyVersionType(string s)
            {
                return (AssemblyVersionType) Enum.Parse(typeof(AssemblyVersionType), s, true);
            }

            private IncrementScheme ReadIncrementScheme(string s)
            {
                return (IncrementScheme) Enum.Parse(typeof(IncrementScheme), s, true);
            }

            private BatchCommandIncrementScheme ReadBatchCommandIncrementScheme(string s)
            {
                return (BatchCommandIncrementScheme) Enum.Parse(typeof(BatchCommandIncrementScheme), s, true);
            }

            private ResetBuildAndRevision ReadResetBuildAndRevision(string s)
            {
                return (ResetBuildAndRevision) Enum.Parse(typeof(ResetBuildAndRevision), s, true);
            }

            private DisplayOptions ReadDisplayOptions()
            {
                var o = new DisplayOptions();
                _reader.MoveToElement();
                if (_reader.IsEmptyElement)
                {
                    _reader.Skip();
                    return o;
                }
                var paramsRead = new bool[8];
                _reader.ReadStartElement();
                _reader.MoveToContent();
                while (_reader.NodeType != XmlNodeType.EndElement)
                {
                    if (_reader.NodeType == XmlNodeType.Element)
                        if (!paramsRead[0] && _reader.LocalName == Tag.Colors)
                        {
                            o.Colors = ReadProjectsListViewColorsConfiguration();
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && _reader.LocalName == Tag.IndentSubProjectItems)
                        {
                            o.IndentSubProjectItems = bool.Parse(_reader.ReadElementString());
                            paramsRead[1] = true;
                        }
                        else if (!paramsRead[2] && _reader.LocalName == Tag.SubProjectIndentation)
                        {
                            o.SubProjectIndentation = int.Parse(_reader.ReadElementString());
                            paramsRead[2] = true;
                        }
                        else if (!paramsRead[3] && _reader.LocalName == Tag.ShowSubProjectRoot)
                        {
                            o.ShowSubProjectRoot = bool.Parse(_reader.ReadElementString());
                            paramsRead[3] = true;
                        }
                        else if (!paramsRead[4] && _reader.LocalName == Tag.ShowEnterpriseTemplateProjectRoot)
                        {
                            o.ShowEnterpriseTemplateProjectRoot = bool.Parse(_reader.ReadElementString());
                            paramsRead[4] = true;
                        }
                        else if (!paramsRead[5] && _reader.LocalName == Tag.ShowEmptyFolders)
                        {
                            o.ShowEmptyFolders = bool.Parse(_reader.ReadElementString());
                            paramsRead[5] = true;
                        }
                        else if (!paramsRead[6] && _reader.LocalName == Tag.ShowNonVersionableProjects)
                        {
                            o.ShowNonVersionableProjects = bool.Parse(_reader.ReadElementString());
                            paramsRead[6] = true;
                        }
                        else if (!paramsRead[7] && _reader.LocalName == Tag.ShowSuccessDialog)
                        {
                            o.ShowSuccessDialog = bool.Parse(_reader.ReadElementString());
                            paramsRead[7] = true;
                        }
                        else
                        {
                            throw InvalidElementName();
                        }
                    else
                        throw InvalidNodeType();
                    _reader.MoveToContent();
                }
                _reader.ReadEndElement();
                return o;
            }

            private ProjectsListViewColorsConfiguration ReadProjectsListViewColorsConfiguration()
            {
                var o = new ProjectsListViewColorsConfiguration();
                _reader.MoveToElement();
                if (_reader.IsEmptyElement)
                {
                    _reader.Skip();
                    return o;
                }
                var paramsRead = new bool[11];
                _reader.ReadStartElement();
                _reader.MoveToContent();
                while (_reader.NodeType != XmlNodeType.EndElement)
                {
                    if (_reader.NodeType == XmlNodeType.Element)
                        if (!paramsRead[0] && _reader.LocalName == Tag.NotModifiedProjectMarked)
                        {
                            o.NotModifiedMarkedAsString = _reader.ReadElementString();
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && _reader.LocalName == Tag.NotModifiedProjectNotMarked)
                        {
                            o.NotModifiedNotMarkedAsString = _reader.ReadElementString();
                            paramsRead[1] = true;
                        }
                        else if (!paramsRead[2] && _reader.LocalName == Tag.ModifiedProjectMarked)
                        {
                            o.ModifiedMarkedAsString = _reader.ReadElementString();
                            paramsRead[2] = true;
                        }
                        else if (!paramsRead[3] && _reader.LocalName == Tag.ModifiedProjectNotMarked)
                        {
                            o.ModifiedNotMarkedAsString = _reader.ReadElementString();
                            paramsRead[3] = true;
                        }
                        else if (!paramsRead[4] && _reader.LocalName == Tag.ProjectWithInvalidVersionMarked)
                        {
                            o.MarkedWithInvalidVersionAsString = _reader.ReadElementString();
                            paramsRead[4] = true;
                        }
                        else if (!paramsRead[5] && _reader.LocalName == Tag.ProjectWithInvalidVersionNotMarked)
                        {
                            o.NotMarkedWithInvalidVersionAsString = _reader.ReadElementString();
                            paramsRead[5] = true;
                        }
                        else if (!paramsRead[6] && _reader.LocalName == Tag.ProjectWithoutVersion)
                        {
                            o.WithoutVersionAsString = _reader.ReadElementString();
                            paramsRead[6] = true;
                        }
                        else if (!paramsRead[7] && _reader.LocalName == Tag.ReportUpdatedVersion)
                        {
                            o.ReportUpdatedVersionAsString = _reader.ReadElementString();
                            paramsRead[7] = true;
                        }
                        else if (!paramsRead[8] && _reader.LocalName == Tag.ReportVersionNotChanged)
                        {
                            o.ReportVersionNotChangedAsString = _reader.ReadElementString();
                            paramsRead[8] = true;
                        }
                        else if (!paramsRead[9] && _reader.LocalName == Tag.ReportVersionUpdateFailed)
                        {
                            o.ReportVersionUpdateFailedAsString = _reader.ReadElementString();
                            paramsRead[9] = true;
                        }
                        else if (!paramsRead[10] && _reader.LocalName == Tag.SubProjectRoot)
                        {
                            o.SubProjectRootAsString = _reader.ReadElementString();
                            paramsRead[10] = true;
                        }
                        else
                        {
                            throw InvalidElementName();
                        }
                    else
                        throw InvalidNodeType();
                    _reader.MoveToContent();
                }
                _reader.ReadEndElement();
                return o;
            }

            private FoldersConfigurations ReadFoldersConfigurations()
            {
                var o = new FoldersConfigurations();
                _reader.MoveToElement();
                if (_reader.IsEmptyElement)
                {
                    _reader.Skip();
                    return o;
                }
                var paramsRead = new bool[2];
                _reader.ReadStartElement();
                _reader.MoveToContent();
                while (_reader.NodeType != XmlNodeType.EndElement)
                {
                    if (_reader.NodeType == XmlNodeType.Element)
                        if (!paramsRead[0] && _reader.LocalName == Tag.SourceSafe)
                        {
                            o.SourceSafeFolder = ReadFolderConfiguration();
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && _reader.LocalName == Tag.Iis)
                        {
                            o.IisFolder = ReadFolderConfiguration();
                            paramsRead[1] = true;
                        }
                        else
                        {
                            throw InvalidElementName();
                        }
                    else
                        throw InvalidNodeType();
                    _reader.MoveToContent();
                }
                _reader.ReadEndElement();
                return o;
            }

            private FolderConfiguration ReadFolderConfiguration()
            {
                var o = new FolderConfiguration();
                var paramsRead = new bool[1];
                while (_reader.MoveToNextAttribute())
                    if (!paramsRead[0] && _reader.LocalName == Attr.IsAvailable)
                    {
                        o.IsAvailable = bool.Parse(_reader.Value);
                        paramsRead[0] = true;
                    }
                    else
                    {
                        InvalidNodeType();
                    }
                _reader.MoveToElement();
                if (_reader.IsEmptyElement)
                {
                    _reader.Skip();
                    return o;
                }
                _reader.ReadStartElement();
                _reader.MoveToContent();
                while (_reader.NodeType != XmlNodeType.EndElement)
                {
                    if (_reader.NodeType == XmlNodeType.Text ||
                        _reader.NodeType == XmlNodeType.CDATA ||
                        _reader.NodeType == XmlNodeType.Whitespace ||
                        _reader.NodeType == XmlNodeType.SignificantWhitespace)
                        o.Folder = _reader.ReadString();
                    else
                        throw InvalidNodeType();
                    _reader.MoveToContent();
                }
                _reader.ReadEndElement();
                return o;
            }

            private ExportConfiguration ReadExportConfiguration()
            {
                var o = new ExportConfiguration();
                _reader.MoveToElement();
                if (_reader.IsEmptyElement)
                {
                    _reader.Skip();
                    return o;
                }
                var paramsRead = new bool[7];
                _reader.ReadStartElement();
                _reader.MoveToContent();
                while (_reader.NodeType != XmlNodeType.EndElement)
                {
                    if (_reader.NodeType == XmlNodeType.Element)
                        if (!paramsRead[0] && _reader.LocalName == Tag.AssemblyVersionTypes)
                        {
                            AssemblyVersionTypeSelection[] avts = null;
                            if (!_reader.IsEmptyElement)
                                avts = ReadAssemblyVersionTypes();
                            else
                                _reader.Skip();
                            Debug.Assert(avts != null && avts.Length == 3);
                            o.AssemblyVersionTypes = avts;
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && _reader.LocalName == Tag.IndentSubItems)
                        {
                            o.IndentSubItems = bool.Parse(_reader.ReadElementString());
                            paramsRead[1] = true;
                        }
                        else if (!paramsRead[2] && _reader.LocalName == Tag.IndentSubItemsBy)
                        {
                            o.IndentSubItemsBy = int.Parse(_reader.ReadElementString());
                            paramsRead[2] = true;
                        }
                        else if (!paramsRead[3] && _reader.LocalName == Tag.ExcludeNonversionableItems)
                        {
                            o.ExcludeNonversionableItems = bool.Parse(_reader.ReadElementString());
                            paramsRead[3] = true;
                        }
                        else if (!paramsRead[4] && _reader.LocalName == Tag.ExportFileFormat)
                        {
                            o.ExportFileFormat = (ExportFileFormat) Enum.Parse(typeof(ExportFileFormat), _reader.ReadElementString());
                            paramsRead[4] = true;
                        }
                        else if (!paramsRead[5] && _reader.LocalName == Tag.CsvSeparator)
                        {
                            o.CsvSeparator = _reader.ReadElementString();
                            paramsRead[5] = true;
                        }
                        else if (!paramsRead[6] && _reader.LocalName == Tag.PrintOptions)
                        {
                            paramsRead[6] = true;
                        }
                        else
                        {
                            throw InvalidElementName();
                        }
                    else
                        throw InvalidNodeType();
                    _reader.MoveToContent();
                }
                _reader.ReadEndElement();
                return o;
            }

            private AssemblyVersionTypeSelection[] ReadAssemblyVersionTypes()
            {
                var o = AssemblyVersionTypeSelection.DefaultSelection;
                var i = 0;
                _reader.ReadStartElement();
                _reader.MoveToContent();
                while (_reader.NodeType != XmlNodeType.EndElement)
                {
                    if (_reader.NodeType == XmlNodeType.Element && _reader.LocalName == Tag.AssemblyVersionType && i < 3)
                    {
                        o[i] = ReadAssemblyVersionTypeSelection();
                        i++;
                    }
                    else
                    {
                        throw InvalidNodeType();
                    }
                    _reader.MoveToContent();
                }
                _reader.ReadEndElement();
                return o;
            }

            private AssemblyVersionTypeSelection ReadAssemblyVersionTypeSelection()
            {
                var o = new AssemblyVersionTypeSelection();
                var paramsRead = new bool[1];
                while (_reader.MoveToNextAttribute())
                    if (!paramsRead[0] && _reader.LocalName == Attr.IsSelected)
                    {
                        o.IsSelected = bool.Parse(_reader.Value);
                        paramsRead[0] = true;
                    }
                    else
                    {
                        throw InvalidNodeType();
                    }
                _reader.MoveToElement();
                if (_reader.IsEmptyElement)
                {
                    _reader.Skip();
                    return o;
                }
                _reader.ReadStartElement();
                _reader.MoveToContent();
                while (_reader.NodeType != XmlNodeType.EndElement)
                {
                    if (_reader.NodeType == XmlNodeType.Text ||
                        _reader.NodeType == XmlNodeType.CDATA ||
                        _reader.NodeType == XmlNodeType.Whitespace ||
                        _reader.NodeType == XmlNodeType.SignificantWhitespace)
                        o.AssemblyVersionType = ReadAssemblyVersionType(_reader.ReadString());
                    else
                        throw InvalidNodeType();
                    _reader.MoveToContent();
                }
                _reader.ReadEndElement();
                return o;
            }

            #endregion // Private method
        }

        #endregion // Reader class

        #region Public methods

        /// <summary>
        ///     Deserializes the configuration.
        /// </summary>
        /// <param name="filename">
        ///     Name of the file to deserialize from.
        /// </param>
        /// <returns>
        ///     Configuration.
        /// </returns>
        public VcbConfiguration Deserialize(string filename)
        {
            using (var reader = new VcbConfigurationXmlReader(filename))
            {
                return reader.ReadConfiguration();
            }
        }

        #endregion // Public methods

        #region Writer class

        #endregion // Writer class

        #region Xml tags and attribute names

        private struct Tag
        {
            public const string VcbConfiguration = "VcbConfiguration";
            public const string MainFormSize = "MainFormSize";
            public const string ListViewColumnWidths = "ListViewColumnWidths";
            public const string ProjectName = "ProjectName";
            public const string CurrentVersion = "CurrentVersion";
            public const string Modified = "Modified";
            public const string ToBeVersion = "ToBeVersion";
            public const string ApplyToAllTabsChecked = "ApplyToAllTabsChecked";
            public const string NumberingOptions = "NumberingOptions";
            public const string SaveModifiedFilesBeforeRunningAddinCommand = "SaveModifiedFilesBeforeRunningAddinCommand";
            public const string DefaultVersionType = "DefaultVersionType";
            public const string IncrementBy = "IncrementBy";
            public const string AllowArbitraryInformationalVersion = "AllowArbitraryInformationalVersion";
            public const string IncludeVCppResourceFiles = "IncludeVCppResourceFiles";
            public const string IncludeSetupProjects = "IncludeSetupProjects";
            public const string GeneratePackageAndProductCodes = "GeneratePackageAndProductCodes";
            public const string ApplyToAllTypes = "ApplyToAllTypes";
            public const string SynchronizeAllVersionTypes = "SynchronizeAllVersionTypes";
            public const string IncrementScheme = "IncrementScheme";
            public const string BatchCommandIncrementScheme = "BatchCommandIncrementScheme";
            public const string UseDateTimeBasedBuildAndRevisionNumbering = "UseDateTimeBasedBuildAndRevisionNumbering";
            public const string ResetBuildOnMajorIncrement = "ResetBuildOnMajorIncrement";
            public const string ResetBuildOnMinorIncrement = "ResetBuildOnMinorIncrement";
            public const string ResetRevisionOnMajorIncrement = "ResetRevisionOnMajorIncrement";
            public const string ResetRevisionOnMinorIncrement = "ResetRevisionOnMinorIncrement";
            public const string ResetRevisionOnBuildIncrement = "ResetRevisionOnBuildIncrement";
            public const string ResetBuildAndRevisionTo = "ResetBuildAndRevisionTo";
            public const string ReplaceAsteriskWithVersionComponents = "ReplaceAsteriskWithVersionComponents";
            public const string DisplayOptions = "DisplayOptions";
            public const string IndentSubProjectItems = "IndentSubProjectItems";
            public const string SubProjectIndentation = "SubProjectIndentation";
            public const string ShowSubProjectRoot = "ShowSubProjectRoot";
            public const string ShowEnterpriseTemplateProjectRoot = "ShowEnterpriseTemplateProjectRoot";
            public const string ShowEmptyFolders = "ShowEmptyFolders";
            public const string ShowNonVersionableProjects = "ShowNonVersionableProjects";
            public const string ShowSuccessDialog = "ShowSuccessDialog";
            public const string Colors = "Colors";
            public const string NotModifiedProjectMarked = "NotModifiedProjectMarked";
            public const string NotModifiedProjectNotMarked = "NotModifiedProjectNotMarked";
            public const string ModifiedProjectMarked = "ModifiedProjectMarked";
            public const string ModifiedProjectNotMarked = "ModifiedProjectNotMarked";
            public const string ProjectWithInvalidVersionMarked = "ProjectWithInvalidVersionMarked";
            public const string ProjectWithInvalidVersionNotMarked = "ProjectWithInvalidVersionNotMarked";
            public const string ProjectWithoutVersion = "ProjectWithoutVersion";
            public const string ReportUpdatedVersion = "ReportUpdatedVersion";
            public const string ReportVersionNotChanged = "ReportVersionNotChanged";
            public const string ReportVersionUpdateFailed = "ReportVersionUpdateFailed";
            public const string SubProjectRoot = "SubProjectRoot";
            public const string FoldersConfigurations = "FoldersConfigurations";
            public const string SourceSafe = "SourceSafe";
            public const string Iis = "IIS";
            public const string ExportConfiguration = "ExportConfiguration";
            public const string AssemblyVersionTypes = "AssemblyVersionTypes";
            public const string AssemblyVersionType = "AssemblyVersionType";
            public const string IndentSubItems = "IndentSubItems";
            public const string IndentSubItemsBy = "IndentSubItemsBy";
            public const string ExcludeNonversionableItems = "ExcludeNonversionableItems";
            public const string ExportFileFormat = "ExportFileFormat";
            public const string CsvSeparator = "CSVSeparator";
            public const string PrintOptions = "PrintOptions";
        }

        private struct Attr
        {
            public const string IsAvailable = "IsAvailable";
            public const string IsSelected = "IsSelected";
        }

        #endregion // Xml tags and attribute names
    }
}