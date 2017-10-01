/*
 * Filename:    ListViewColumnWidths.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: GUI form listview column widths.
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
    [Serializable]
    public class ListViewColumnWidths
    {
        private int _currentVersion;
        private int _modified;

        private int _projectName;
        private int _toBeVersion;

        public ListViewColumnWidths()
        {
            _projectName = 175;
            _currentVersion = 85;
            _modified = 120;
            _toBeVersion = 85;
        }

        public int ProjectName
        {
            get => _projectName;
            set => _projectName = value;
        }

        public int CurrentVersion
        {
            get => _currentVersion;
            set => _currentVersion = value;
        }

        public int Modified
        {
            get => _modified;
            set => _modified = value;
        }

        public int ToBeVersion
        {
            get => _toBeVersion;
            set => _toBeVersion = value;
        }
    }
}