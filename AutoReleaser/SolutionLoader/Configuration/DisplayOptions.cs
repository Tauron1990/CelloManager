/*
 * Filename:    DisplayOptions.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Display options for main GUI form of the tool.
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

namespace AutoReleaser.SolutionLoader.Configuration
{
    /// <summary>
    ///     Display options for main GUI form of the tool.
    /// </summary>
    [Serializable]
    public class DisplayOptions : ICloneable
    {
        private bool _displaySuccessDialog;
        private bool _indentSubProjectItems;


        private ProjectsListViewColorsConfiguration _listViewColors;
        private bool _showEmptyFolders;
        private bool _showEnterpriseTemplateProjectRoot;
        private bool _showNonVersionableProjects;
        private bool _showSubprojectRoot;
        private int _subProjectsIndentation;

        public DisplayOptions()
        {
            _listViewColors = ProjectsListViewColorsConfiguration.Default;
            _indentSubProjectItems = true;
            _subProjectsIndentation = 10;
            _showSubprojectRoot = true;
            _showEnterpriseTemplateProjectRoot = true;
            _showEmptyFolders = true;
            _showNonVersionableProjects = true;
            _displaySuccessDialog = true;
        }

        public ProjectsListViewColorsConfiguration Colors
        {
            get => _listViewColors;
            set => _listViewColors = value;
        }

        public bool IndentSubProjectItems
        {
            get => _indentSubProjectItems;
            set => _indentSubProjectItems = value;
        }

        public int SubProjectIndentation
        {
            get => _subProjectsIndentation;
            set => _subProjectsIndentation = value;
        }

        public bool ShowSubProjectRoot
        {
            get => _showSubprojectRoot;
            set => _showSubprojectRoot = value;
        }

        public bool ShowEnterpriseTemplateProjectRoot
        {
            get => _showEnterpriseTemplateProjectRoot;
            set => _showEnterpriseTemplateProjectRoot = value;
        }

        public bool ShowEmptyFolders
        {
            get => _showEmptyFolders;
            set => _showEmptyFolders = value;
        }

        public bool ShowNonVersionableProjects
        {
            get => _showNonVersionableProjects;
            set => _showNonVersionableProjects = value;
        }

        public bool ShowSuccessDialog
        {
            get => _displaySuccessDialog;
            set => _displaySuccessDialog = value;
        }

        #region ICloneable implementation

        object ICloneable.Clone()
        {
            return Clone();
        }

        public DisplayOptions Clone()
        {
            var newOptions = (DisplayOptions) MemberwiseClone();
            newOptions.Colors = Colors.Clone();
            return newOptions;
        }

        #endregion // ICloneable implementation
    }
}