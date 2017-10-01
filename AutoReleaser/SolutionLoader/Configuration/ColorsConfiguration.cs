/*
 * Filename:    ColorsConfiguration.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Configuration part used to persist listview colors. 
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
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace AutoReleaser.SolutionLoader.Configuration
{
    /// <summary>
    ///     Configuration part used to persist listview colors.
    /// </summary>
    [Serializable]
    public class ProjectsListViewColorsConfiguration : ICloneable
    {
        public static readonly ProjectsListViewColorsConfiguration Default = new ProjectsListViewColorsConfiguration();
        private Color _invalidVersionMarked;
        private Color _invalidVersionNotMarked;
        private Color _modifiedMarked;
        private Color _modifiedNotMarked;


        private Color _notModifiedMarked;
        private Color _notModifiedNotMarked;
        private Color _noVersion;
        private Color _subProjectRoot;
        private Color _updatedVersion;
        private Color _versionNotChanged;
        private Color _versionUpdateFailed;

        public ProjectsListViewColorsConfiguration()
        {
            _notModifiedMarked = SystemColors.WindowTextColor;
            _notModifiedNotMarked = SystemColors.GrayTextColor;
            _modifiedMarked = Colors.Green;
            _modifiedNotMarked = Colors.LimeGreen;
            _invalidVersionMarked = Colors.Red;
            _invalidVersionNotMarked = Colors.LightCoral;
            _noVersion = Colors.Orange;
            _updatedVersion = Colors.Blue;
            _versionNotChanged = SystemColors.GrayTextColor;
            _versionUpdateFailed = Colors.Red;
            _subProjectRoot = Colors.SlateGray;
        }

        [XmlIgnore]
        public Color NotModifiedMarked
        {
            get => _notModifiedMarked;
            set => _notModifiedMarked = value;
        }

        [XmlElement("NotModifiedProjectMarked")]
        public string NotModifiedMarkedAsString
        {
            get => AsString(_notModifiedMarked);
            set => _notModifiedMarked = FromString(value);
        }

        [XmlIgnore]
        public Color NotModifiedNotMarked
        {
            get => _notModifiedNotMarked;
            set => _notModifiedNotMarked = value;
        }

        [XmlElement("NotModifiedProjectNotMarked")]
        public string NotModifiedNotMarkedAsString
        {
            get => AsString(_notModifiedNotMarked);
            set => _notModifiedNotMarked = FromString(value);
        }

        [XmlIgnore]
        public Color ModifiedMarked
        {
            get => _modifiedMarked;
            set => _modifiedMarked = value;
        }

        [XmlElement("ModifiedProjectMarked")]
        public string ModifiedMarkedAsString
        {
            get => AsString(_modifiedMarked);
            set => _modifiedMarked = FromString(value);
        }

        [XmlIgnore]
        public Color ModifiedNotMarked
        {
            get => _modifiedNotMarked;
            set => _modifiedNotMarked = value;
        }

        [XmlElement("ModifiedProjectNotMarked")]
        public string ModifiedNotMarkedAsString
        {
            get => AsString(_modifiedNotMarked);
            set => _modifiedNotMarked = FromString(value);
        }

        [XmlIgnore]
        public Color InvalidVersionMarked
        {
            get => _invalidVersionMarked;
            set => _invalidVersionMarked = value;
        }

        [XmlElement("ProjectWithInvalidVersionMarked")]
        public string MarkedWithInvalidVersionAsString
        {
            get => AsString(_invalidVersionMarked);
            set => _invalidVersionMarked = FromString(value);
        }

        [XmlIgnore]
        public Color InvalidVersionNotMarked
        {
            get => _invalidVersionNotMarked;
            set => _invalidVersionNotMarked = value;
        }

        [XmlElement("ProjectWithInvalidVersionNotMarked")]
        public string NotMarkedWithInvalidVersionAsString
        {
            get => AsString(_invalidVersionNotMarked);
            set => _invalidVersionNotMarked = FromString(value);
        }

        [XmlIgnore]
        public Color NoVersion
        {
            get => _noVersion;
            set => _noVersion = value;
        }

        [XmlElement("ProjectWithoutVersion")]
        public string WithoutVersionAsString
        {
            get => AsString(_noVersion);
            set => _noVersion = FromString(value);
        }

        [XmlIgnore]
        public Color ReportUpdatedVersion
        {
            get => _updatedVersion;
            set => _updatedVersion = value;
        }

        [XmlElement("ReportUpdatedVersion")]
        public string ReportUpdatedVersionAsString
        {
            get => AsString(_updatedVersion);
            set => _updatedVersion = FromString(value);
        }

        [XmlIgnore]
        public Color ReportVersionNotChanged
        {
            get => _versionNotChanged;
            set => _versionNotChanged = value;
        }

        [XmlElement("ReportVersionNotChanged")]
        public string ReportVersionNotChangedAsString
        {
            get => AsString(_versionNotChanged);
            set => _versionNotChanged = FromString(value);
        }

        [XmlIgnore]
        public Color ReportVersionUpdateFailed
        {
            get => _versionUpdateFailed;
            set => _versionUpdateFailed = value;
        }

        [XmlElement("ReportVersionUpdateFailed")]
        public string ReportVersionUpdateFailedAsString
        {
            get => AsString(_versionUpdateFailed);
            set => _versionUpdateFailed = FromString(value);
        }

        [XmlIgnore]
        public Color SubProjectRoot
        {
            get => _subProjectRoot;
            set => _subProjectRoot = value;
        }

        [XmlElement("SubProjectRoot")]
        public string SubProjectRootAsString
        {
            get => AsString(_subProjectRoot);
            set => _subProjectRoot = FromString(value);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public ProjectsListViewColorsConfiguration Clone()
        {
            return (ProjectsListViewColorsConfiguration) MemberwiseClone();
        }

        public override bool Equals(object other)
        {
            if (other == null)
                return false;
            if (GetType() != other.GetType())
                return false;
            var oplvcc = (ProjectsListViewColorsConfiguration) other;
            if (NotModifiedMarked != oplvcc.NotModifiedMarked)
                return false;
            if (NotModifiedNotMarked != oplvcc.NotModifiedNotMarked)
                return false;
            if (ModifiedMarked != oplvcc.ModifiedMarked)
                return false;
            if (ModifiedNotMarked != oplvcc.ModifiedNotMarked)
                return false;
            if (InvalidVersionMarked != oplvcc.InvalidVersionMarked)
                return false;
            if (InvalidVersionNotMarked != oplvcc.InvalidVersionNotMarked)
                return false;
            if (NoVersion != oplvcc.NoVersion)
                return false;
            if (ReportUpdatedVersion != oplvcc.ReportUpdatedVersion)
                return false;
            if (ReportVersionNotChanged != oplvcc.ReportVersionNotChanged)
                return false;
            if (ReportVersionUpdateFailed != oplvcc.ReportVersionUpdateFailed)
                return false;
            if (SubProjectRoot != oplvcc.SubProjectRoot)
                return false;
            return true;
        }

        private string AsString(Color c)
        {
            return c.ToString();
        }

        private Color FromString(string s)
        {
            // ReSharper disable once PossibleNullReferenceException
            return (Color) ColorConverter.ConvertFromString(s);
        }

        public static bool operator ==(ProjectsListViewColorsConfiguration plvcc1, ProjectsListViewColorsConfiguration plvcc2)
        {
            if (ReferenceEquals(null, plvcc1) || ReferenceEquals(null, plvcc2)) return ReferenceEquals(plvcc1, plvcc2);
            return plvcc1.Equals(plvcc2);
        }

        public static bool operator !=(ProjectsListViewColorsConfiguration plvcc1, ProjectsListViewColorsConfiguration plvcc2)
        {
            return !(plvcc1 == plvcc2);
        }
    }
}