/*
 * Filename:    FolderConfiguration.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Configuration of folders for SourceSafe and IIS.
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
using System.Xml.Serialization;

namespace AutoReleaser.SolutionLoader.Configuration
{
    /// <summary>
    ///     Contains configuration of folders for SourceSafe and IIS.
    /// </summary>
    [Serializable]
    public class FolderConfiguration : ICloneable
    {
        private string _folder;

        private bool _isAvailable;

        /// <summary>
        ///     Creates default instance.
        /// </summary>
        public FolderConfiguration()
        {
            _isAvailable = true;
            _folder = string.Empty;
        }

        /// <summary>
        ///     Gets/sets a flag if this folder (i.e. component using it) is
        ///     available at all.
        /// </summary>
        [XmlAttribute]
        public bool IsAvailable
        {
            get => _isAvailable;
            set => _isAvailable = value;
        }

        /// <summary>
        ///     Gets/sets a path to the folder.
        /// </summary>
        [XmlText]
        public string Folder
        {
            get
            {
                Debug.Assert(_folder.Length == 0 || Directory.Exists(_folder));
                return _folder;
            }
            set
            {
                Debug.Assert(value.Length == 0 || Directory.Exists(value));
                _folder = value;
            }
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public FolderConfiguration Clone()
        {
            return (FolderConfiguration) MemberwiseClone();
        }
    }


    [Serializable]
    [XmlRoot("Folders")]
    public class FoldersConfigurations : ICloneable
    {
        private FolderConfiguration _iisFolder;

        private FolderConfiguration _sourceSafeFolder;

        public FoldersConfigurations()
        {
            _sourceSafeFolder = new FolderConfiguration();
            _iisFolder = new FolderConfiguration();
        }

        [XmlElement("SourceSafe")]
        public FolderConfiguration SourceSafeFolder
        {
            get => _sourceSafeFolder;
            set => _sourceSafeFolder = value;
        }

        [XmlElement("IIS")]
        public FolderConfiguration IisFolder
        {
            get => _iisFolder;
            set => _iisFolder = value;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public FoldersConfigurations Clone()
        {
            var clone = new FoldersConfigurations
            {
                SourceSafeFolder = SourceSafeFolder.Clone(),
                IisFolder = IisFolder.Clone()
            };
            return clone;
        }
    }
}