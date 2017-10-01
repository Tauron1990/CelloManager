/*
 * Filename:    VcbConfiguration.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Tool configuration.
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
using System.Xml.Serialization;

namespace AutoReleaser.SolutionLoader.Configuration
{
    [Serializable]
    public class VcbConfiguration : ICloneable
    {
        private bool _applyToAllTabsChecked;
        private bool _configurationFileRead;
        private DisplayOptions _displayOptions;
        private ExportConfiguration _exportConfiguration;
        private FoldersConfigurations _foldersConfigurations;
        private ListViewColumnWidths _listViewColumnWidths;
        private NumberingOptions _numberingOptions;

        public VcbConfiguration()
        {
            _listViewColumnWidths = new ListViewColumnWidths();
            _applyToAllTabsChecked = true;
            _numberingOptions = new NumberingOptions();
            _displayOptions = new DisplayOptions();
            _foldersConfigurations = new FoldersConfigurations();
            _configurationFileRead = false;
            _exportConfiguration = new ExportConfiguration();
        }

        //public System.Drawing.Size MainFormSize {
        //    get { return _size; }
        //    set { _size = value; }
        //}

        public ListViewColumnWidths ListViewColumnWidths
        {
            get => _listViewColumnWidths;
            set => _listViewColumnWidths = value;
        }

        public bool ApplyToAllTabsChecked
        {
            get => _applyToAllTabsChecked;
            set => _applyToAllTabsChecked = value;
        }

        public NumberingOptions NumberingOptions
        {
            get => _numberingOptions;
            set => _numberingOptions = value;
        }

        public DisplayOptions DisplayOptions
        {
            get => _displayOptions;
            set => _displayOptions = value;
        }

        public FoldersConfigurations FoldersConfigurations
        {
            get => _foldersConfigurations;
            set => _foldersConfigurations = value;
        }

        public ExportConfiguration ExportConfiguration
        {
            get => _exportConfiguration;
            set => _exportConfiguration = value;
        }

        /// <summary>
        ///     Flag used to identify if configuration has been read from file.
        /// </summary>
        [XmlIgnore]
        public bool ConfigurationFileRead
        {
            get => _configurationFileRead;
            set => _configurationFileRead = value;
        }

        #region ICloneable implementation

        object ICloneable.Clone()
        {
            return Clone();
        }

        private VcbConfiguration Clone()
        {
            var newConfig = (VcbConfiguration) MemberwiseClone();
            newConfig.NumberingOptions = NumberingOptions.Clone();
            newConfig.DisplayOptions = DisplayOptions.Clone();
            newConfig.FoldersConfigurations = FoldersConfigurations.Clone();
            return newConfig;
        }

        #endregion // ICloneable implementation
    }
}