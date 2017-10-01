/*
 * Filename:    ProjectVersion.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Wrapper arround version.
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
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;
using AutoReleaser.SolutionLoader.Configuration;
using AutoReleaser.SolutionLoader.Resources;
using AutoReleaser.SolutionLoader.Util;

namespace AutoReleaser.SolutionLoader.Versions
{
    /// <summary>
    ///     <c>ProjectVersion</c> is a wrapper arround <c>Version</c> class
    ///     adding increment functionality and supporting <c>Empty</c> (i.e.
    ///     not defined) version.
    /// </summary>
    public class ProjectVersion : IComparable, ICloneable
    {
        public enum VersionComponent
        {
            Major,
            Minor,
            Build,
            Revision
        }

        /// <summary>
        ///     Maximum version for a component.
        /// </summary>
        private const int MaxVersion = ushort.MaxValue;

        #region Type constructor

        static ProjectVersion()
        {
            var resources = Shared.ResourceManager;
            Debug.Assert(resources != null);

            var txtInvalidVersionString = resources.GetString("Invalid version string");
            var txtInvalidVersionPattern = resources.GetString("Invalid version pattern");
            TxtVersionMustConsistOfAtLeastNComponents = resources.GetString("Version must consist of at least N components");
            TxtVersionMustConsistOfAtMostNComponents = resources.GetString("Version must consist of at most N components");
            TxtMustNotContainNegativeIntegers = resources.GetString("Version must not contain negative integers");
            TxtMustBeNonNegativeIntegersOrAsterisk = resources.GetString("Version may consist of non-negative integers or a single asterisk character");
            TxtMustBeNonNegativeIntegers = resources.GetString("Version may consist of non-negative integers");
            TxtAsteriskMustBeLast = resources.GetString("Asterisk may appear only at the end of version string");
            TxtVersionMustNotEndWithDot = resources.GetString("Version must not end with dot");
            TxtNoAsteriskAllowed = resources.GetString("Asterisk not allowed");
            TxtMustBeIntegerSmallerThanMaxValue = string.Format(resources.GetString("Version must be smaller than") ?? throw new InvalidOperationException(), MaxVersion + 1);

            Debug.Assert(txtInvalidVersionString != null);
            Debug.Assert(txtInvalidVersionPattern != null);
            Debug.Assert(TxtVersionMustConsistOfAtLeastNComponents != null);
            Debug.Assert(TxtVersionMustConsistOfAtMostNComponents != null);
            Debug.Assert(TxtMustNotContainNegativeIntegers != null);
            Debug.Assert(TxtMustBeNonNegativeIntegersOrAsterisk != null);
            Debug.Assert(TxtMustBeNonNegativeIntegers != null);
            Debug.Assert(TxtAsteriskMustBeLast != null);
            Debug.Assert(TxtVersionMustNotEndWithDot != null);
            Debug.Assert(TxtNoAsteriskAllowed != null);
            Debug.Assert(TxtMustBeIntegerSmallerThanMaxValue != null);
        }

        #endregion // Type constructor

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <c>ProjectVersion</c> class
        ///     with a non-defined (empty) version.
        /// </summary>
        private ProjectVersion()
        {
            Version = new ListDictionary();
            _valid = true;
        }

        public ProjectVersion(string version, AssemblyVersionType versionType) : this()
        {
            _originalString = version;
            if (version.IndexOf(',') != -1) version = version.Replace(',', '.');
            _valid = IsValidVersionString(version, versionType, ProjectType.CSharpProject);
            if (_valid)
                SplitComponents(version);
        }

        /// <summary>
        ///     Initializes a new instance of the <c>ProjectVersion</c> class
        ///     with the specified major, minor, build, and revision numbers.
        /// </summary>
        /// <param name="major">
        ///     The major version number.
        /// </param>
        /// <param name="minor">
        ///     The minor version number.
        /// </param>
        /// <param name="build">
        ///     The build number.
        /// </param>
        /// <param name="revision">
        ///     The revision number.
        /// </param>
        private ProjectVersion(int major, int minor, int build, int revision) : this(major, minor, build)
        {
            Version[VersionComponent.Revision] = revision;
        }

        /// <summary>
        ///     Initializes a new instance of the <c>ProjectVersion</c> class
        ///     with the specified major, minor and build numbers.
        /// </summary>
        /// <param name="major">
        ///     The major version number.
        /// </param>
        /// <param name="minor">
        ///     The minor version number.
        /// </param>
        /// <param name="build">
        ///     The build number.
        /// </param>
        private ProjectVersion(int major, int minor, int build) : this(major, minor)
        {
            Version[VersionComponent.Build] = build;
        }

        /// <summary>
        ///     Initializes a new instance of the <c>ProjectVersion</c> class
        ///     with the specified major and minor numbers.
        /// </summary>
        /// <param name="major">
        ///     The major version number.
        /// </param>
        /// <param name="minor">
        ///     The minor version number.
        /// </param>
        private ProjectVersion(int major, int minor) : this()
        {
            Version[VersionComponent.Major] = major;
            Version[VersionComponent.Minor] = minor;
        }

        /// <summary>
        ///     Copy constructor.
        /// </summary>
        /// <param name="version">
        ///     <c>ProjectVersion</c> to copy.
        /// </param>
        private ProjectVersion(ProjectVersion version) : this()
        {
            foreach (VersionComponent vc in version.Version.Keys) Version[vc] = (int) version.Version[vc];
            _valid = version._valid;
            _originalString = version._originalString;
        }

        #endregion // Constructors

        #region IComparable interface implementation

        /// <summary>
        ///     IComparable interface implementation.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        int IComparable.CompareTo(object obj)
        {
            return CompareTo((ProjectVersion) obj);
        }

        private int CompareTo(ProjectVersion other)
        {
            var vc1 = ComparableComponents;
            var vc2 = other.ComparableComponents;
            for (var i = 0; i < 4; i++)
            {
                if (vc1[i] < vc2[i]) return -1;
                if (vc1[i] > vc2[i]) return +1;
            }
            return 0;
        }

        #endregion // IComparable interface implementation

        #region ICloneable interface implementation

        object ICloneable.Clone()
        {
            return Clone();
        }

        public ProjectVersion Clone()
        {
            return new ProjectVersion(this);
        }


        public ProjectVersion Clone(int build, int revision, bool createRevision)
        {
            Debug.Assert(Version.Count >= 2);
            if (Version.Count == 2)
                return new ProjectVersion(this[VersionComponent.Major], this[VersionComponent.Minor]);
            if (Version.Count == 4 || this[VersionComponent.Build] == MaxVersion && createRevision)
                return new ProjectVersion(this[VersionComponent.Major], this[VersionComponent.Minor], build, revision);
            return new ProjectVersion(this[VersionComponent.Major], this[VersionComponent.Minor], build);
        }

        #endregion // ICloneable interface implementation

        #region Public methods

        /// <summary>
        ///     Increments the version according to numbering scheme defined in
        ///     configuration.
        /// </summary>
        /// <param name="numberingOptions">
        ///     <c>NumberingOptions</c> that define which component of the version
        ///     should be incremented.
        /// </param>
        public void Increment(NumberingOptions numberingOptions)
        {
            switch (numberingOptions.IncrementScheme)
            {
                case IncrementScheme.IncrementMajorVersion:
                    IncrementComponent(VersionComponent.Major, numberingOptions);
                    break;
                case IncrementScheme.IncrementMinorVersion:
                    IncrementComponent(VersionComponent.Minor, numberingOptions);
                    break;
                case IncrementScheme.IncrementBuild:
                    IncrementComponent(VersionComponent.Build, numberingOptions);
                    break;
                case IncrementScheme.IncrementRevision:
                    IncrementComponent(VersionComponent.Revision, numberingOptions);
                    break;
            }
        }

        /// <summary>
        ///     Increments the version using configuration settings. Numerical
        ///     overflow increments the higher component.
        /// </summary>
        /// <param name="toIncrement">
        ///     <c>VersionComponent</c> to increment.
        /// </param>
        /// <param name="numberingOptions">
        ///     <c>NumberingOptions</c> which defines the scheme for increment.
        /// </param>
        private void IncrementComponent(VersionComponent toIncrement, NumberingOptions numberingOptions)
        {
            if (numberingOptions.UseDateTimeBasedBuildAndRevisionNumbering)
                IncrementUsingDateTimeBasedBuildAndRevisionNumbering(toIncrement, numberingOptions);
            else
                IncrementVersionStandard(toIncrement, numberingOptions);
        }

        #endregion // Public methods

        #region Public overrides

        /// <summary>
        ///     Converts the value of this instance to its equivalent <c>String</c>
        ///     representation.
        /// </summary>
        /// <returns>
        ///     The string representation of the values of the major, minor,
        ///     build, and revision components of this instance.
        /// </returns>
        public override string ToString()
        {
            return ToString(false);
        }

        /// <summary>
        ///     Converts the value of this instance to its equivalent <c>String</c>
        ///     representation.
        /// </summary>
        /// <param name="displayAllComponents">
        ///     Flag indicating if all versions (including Build and Revision)
        ///     should be included. If this flag is set to <c>true</c> and Build
        ///     and/or Revision are missing, they are substituted by asterisk.
        /// </param>
        /// <returns>
        ///     The string representation of the values of the major, minor,
        ///     build, and revision components of this instance.
        /// </returns>
        private string ToString(bool displayAllComponents)
        {
            if (this == Empty)
                return _originalString;
            var result = new StringBuilder();
            result.AppendFormat("{0}.{1}", (int) Version[VersionComponent.Major], (int) Version[VersionComponent.Minor]);
            var asteriskInput = false;
            if (Version[VersionComponent.Build] != null)
            {
                var build = (int) Version[VersionComponent.Build];
                if (build < MaxVersion)
                {
                    Debug.Assert(build >= 0);
                    result.AppendFormat(".{0}", build);
                }
                else
                {
                    result.Append(".*");
                    asteriskInput = true;
                }
            }
            else if (displayAllComponents)
            {
                result.Append(".*");
            }
            if (Version[VersionComponent.Revision] != null)
            {
                var revision = (int) Version[VersionComponent.Revision];
                if (revision < MaxVersion)
                {
                    Debug.Assert(revision >= 0);
                    result.AppendFormat(".{0}", revision);
                }
                else if (!asteriskInput)
                {
                    result.Append(".*");
                }
            }
            else if (displayAllComponents)
            {
                result.Append(".*");
            }
            return result.ToString();
        }

        /// <summary>
        ///     Returns a value indicating whether this instance is equal to a
        ///     specified object.
        /// </summary>
        /// <param name="obj">
        ///     An object to compare with this instance, or a null reference.
        /// </param>
        /// <returns>
        ///     <c>true</c> if this instance and <c>obj</c> are both
        ///     <c>ProjectVersion</c> objects, and every component of this
        ///     instance matches the corresponding component of <c>obj</c>;
        ///     otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            var otherPv = (ProjectVersion) obj;
            return CompareTo(otherPv) == 0;
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode()
        {
            return 0;
        }

        #endregion Public overrides

        #region Private properties

        /// <summary>
        ///     Gets the internal <c>Version</c> list.
        /// </summary>
        private ListDictionary Version { get; }

        private int this[VersionComponent component] => (int) Version[component];

        #endregion // Private properties

        #region Private methods

        /// <summary>
        ///     Splits version components.
        /// </summary>
        /// <param name="version"></param>
        private void SplitComponents(string version)
        {
            var splitVersion = version.Split('.');
            Debug.Assert(splitVersion.Length > 1);
            Version[VersionComponent.Major] = int.Parse(splitVersion[0]);
            if (splitVersion.Length > 1)
                Version[VersionComponent.Minor] = int.Parse(splitVersion[1]);
            if (splitVersion.Length > 2)
                Version[VersionComponent.Build] = splitVersion[2] == "*" ? MaxVersion : int.Parse(splitVersion[2]);
            if (splitVersion.Length > 3)
                Version[VersionComponent.Revision] = splitVersion[3] == "*" ? MaxVersion : int.Parse(splitVersion[3]);
        }

        /// <summary>
        ///     Increments version.
        /// </summary>
        /// <param name="toIncrement">
        ///     Version component to increment.
        /// </param>
        /// <param name="numberingOptions">
        ///     Numbering options from the configuration.
        /// </param>
        private void IncrementVersionStandard(VersionComponent toIncrement, NumberingOptions numberingOptions)
        {
            Debug.Assert(!numberingOptions.UseDateTimeBasedBuildAndRevisionNumbering);
            CreateMissingBuildAndRevision(numberingOptions);
            var incrementStep = numberingOptions.IncrementBy;
            var overflow = false;
            if (toIncrement == VersionComponent.Revision)
                if (Version[VersionComponent.Revision] != null && (int) Version[VersionComponent.Revision] != MaxVersion)
                {
                    overflow = true;
                    Version[VersionComponent.Revision] = IncrementIntWithOverflow(this[VersionComponent.Revision], incrementStep, ref overflow, (int) numberingOptions.ResetBuildAndRevisionTo);
                }
            if (toIncrement == VersionComponent.Build)
            {
                overflow = true;
                if (numberingOptions.ResetRevisionOnBuildIncrement && Version[VersionComponent.Revision] != null)
                    Version[VersionComponent.Revision] = (int) numberingOptions.ResetBuildAndRevisionTo;
            }
            if (overflow)
                if (Version[VersionComponent.Build] != null && (int) Version[VersionComponent.Build] != MaxVersion)
                    Version[VersionComponent.Build] = IncrementIntWithOverflow((int) Version[VersionComponent.Build], incrementStep, ref overflow, (int) numberingOptions.ResetBuildAndRevisionTo);
                else overflow = false;
            if (toIncrement == VersionComponent.Minor)
            {
                overflow = true;
                if (numberingOptions.ResetBuildOnMinorIncrement && Version[VersionComponent.Build] != null)
                    Version[VersionComponent.Build] = (int) numberingOptions.ResetBuildAndRevisionTo;
                if (numberingOptions.ResetRevisionOnMinorIncrement && Version[VersionComponent.Revision] != null)
                    Version[VersionComponent.Revision] = (int) numberingOptions.ResetBuildAndRevisionTo;
            }
            Version[VersionComponent.Minor] = IncrementIntWithOverflow((int) Version[VersionComponent.Minor], incrementStep, ref overflow, 0);
            if (toIncrement == VersionComponent.Major)
            {
                overflow = true;
                // incrementing Major will automatically reset Minor
                Version[VersionComponent.Minor] = 0;
                if (numberingOptions.ResetBuildOnMinorIncrement && Version[VersionComponent.Build] != null)
                    Version[VersionComponent.Build] = (int) numberingOptions.ResetBuildAndRevisionTo;
                if (numberingOptions.ResetRevisionOnMinorIncrement && Version[VersionComponent.Revision] != null)
                    Version[VersionComponent.Revision] = (int) numberingOptions.ResetBuildAndRevisionTo;
            }
            Version[VersionComponent.Major] = IncrementIntWithOverflow((int) Version[VersionComponent.Major], incrementStep, ref overflow, 0);
            if (overflow) throw new VersionOverflowException(VersionComponent.Major);
        }


        private void CreateMissingBuildAndRevision(NumberingOptions numberingOptions)
        {
            if (!numberingOptions.ReplaceAsteriskWithVersionComponents) return;

            if (Version[VersionComponent.Revision] == null && Version[VersionComponent.Build] != null && (int) Version[VersionComponent.Build] == MaxVersion
                || Version[VersionComponent.Revision] != null && (int) Version[VersionComponent.Revision] == MaxVersion) Version[VersionComponent.Revision] = (int) numberingOptions.ResetBuildAndRevisionTo;
            if (Version[VersionComponent.Build] != null && (int) Version[VersionComponent.Build] == MaxVersion) Version[VersionComponent.Build] = (int) numberingOptions.ResetBuildAndRevisionTo;
        }

        /// <summary>
        ///     Increments version using date &amp; time based build/revision
        ///     schema.
        /// </summary>
        /// <param name="toIncrement">
        ///     Version component to increment.
        /// </param>
        /// <param name="numberingOptions">
        ///     Numbering options from the configuration.
        /// </param>
        private void IncrementUsingDateTimeBasedBuildAndRevisionNumbering(VersionComponent toIncrement, NumberingOptions numberingOptions)
        {
            Debug.Assert(numberingOptions.UseDateTimeBasedBuildAndRevisionNumbering);
            var incrementStep = numberingOptions.IncrementBy;
            var overflow = toIncrement == VersionComponent.Minor;
            Version[VersionComponent.Minor] = IncrementIntWithOverflow((int) Version[VersionComponent.Minor], incrementStep, ref overflow, 0);
            if (toIncrement == VersionComponent.Major)
            {
                overflow = true;
                // incrementing Major automatically resets Minor
                Version[VersionComponent.Minor] = 0;
            }
            Version[VersionComponent.Major] = IncrementIntWithOverflow((int) Version[VersionComponent.Major], incrementStep, ref overflow, 0);
            if (overflow) throw new VersionOverflowException(VersionComponent.Major);
        }


        private int IncrementIntWithOverflow(int toIncrement, int incrementStep, ref bool overflow, int resetValue)
        {
            if (!overflow) return toIncrement;

            if (toIncrement < 0)
            {
                overflow = false;
                return resetValue;
            }
            toIncrement += incrementStep;
            // if value to increment has the largest value, then it 
            // should be reset and overflow is forwarded to higher version
            if (toIncrement >= MaxVersion) return resetValue;
            overflow = false;
            return toIncrement;
        }

        /// <summary>
        ///     Gets an array of integers representing components, that may be
        ///     used for version comparisons.
        /// </summary>
        private int[] ComparableComponents
        {
            get
            {
                if (Version.Count == 0)
                    return new[] {-1, -1, -1, -1};
                int[] components = {0, 0, 0, 0};
                Debug.Assert(Version.Count >= 2 && Version.Count <= 4);
                components[0] = (int) Version[VersionComponent.Major];
                components[1] = (int) Version[VersionComponent.Minor];

                if (Version.Count <= 2) return components;

                components[2] = (int) Version[VersionComponent.Build];
                if (Version.Count > 3) components[3] = (int) Version[VersionComponent.Revision];
                else if (components[2] == MaxVersion) components[3] = MaxVersion;
                else
                    components[3] = 0;
                return components;
            }
        }

        #endregion // Private methods

        #region Private fields

        private readonly string _originalString = "";

        private static readonly string TxtVersionMustConsistOfAtLeastNComponents;
        private static readonly string TxtVersionMustConsistOfAtMostNComponents;
        private static readonly string TxtMustNotContainNegativeIntegers;
        private static readonly string TxtMustBeNonNegativeIntegersOrAsterisk;
        private static readonly string TxtMustBeNonNegativeIntegers;
        private static readonly string TxtAsteriskMustBeLast;
        private static readonly string TxtNoAsteriskAllowed;

        private static readonly string TxtMustBeIntegerSmallerThanMaxValue;

        private static readonly string TxtVersionMustNotEndWithDot;

        #endregion Private fields

        #region Public static properties

        private readonly bool _valid;

        public static readonly ProjectVersion Empty = new ProjectVersion();

        public static readonly ProjectVersion MinValue = new ProjectVersion(0, 0, 0, 0);

        #endregion // Public static properties

        #region Public static methods

        /// <summary>
        ///     Applies a pattern to the version.
        /// </summary>
        /// <param name="pattern">
        ///     Pattern to apply. Pattern may contain '*' and '+' wildcards.
        /// </param>
        /// <param name="version">
        ///     Version onto which pattern has to be applied.
        /// </param>
        /// <param name="buildAndRevisionResetValue">
        ///     Reset value of Build and Revision.
        /// </param>
        /// <returns>
        ///     String with new version.
        /// </returns>
        public static string ApplyVersionPattern(string pattern, string version, int buildAndRevisionResetValue)
        {
            Debug.Assert(IsValidPattern(pattern));
            var patternSections = pattern.Split('.');
            // replace "+" with "+1" for consistency
            for (var l = 0; l < patternSections.Length; l++)
                if (patternSections[l] == "+")
                    patternSections[l] = "+1";
            var versionSections = version.Split('.');
            Debug.Assert(patternSections.Length >= versionSections.Length);
            var result = new StringCollection();
            var versionLength = versionSections.Length;
            var i = 0;
            var moreNumbersInPattern = MoreNumbersInPattern(patternSections, 0);
            // first apply pattern sections to existent version sections
            while (i < versionLength)
            {
                if (patternSections[i].StartsWith("+"))
                {
                    var increment = int.Parse(patternSections[i]);
                    try
                    {
                        result.Add(IncrementStringInteger(versionSections[i], increment));
                    }
                    catch (OverflowException)
                    {
                        var vc = (VersionComponent) Enum.GetValues(typeof(VersionComponent)).GetValue(i);
                        throw new VersionOverflowException(vc);
                    }
                }
                else if (patternSections[i] != "*")
                {
                    result.Add(patternSections[i]);
                }
                else
                {
                    // update moreNumbersInPatern (only if true)
                    if (moreNumbersInPattern)
                    {
                        Debug.Assert(patternSections[i] == "*");
                        moreNumbersInPattern = MoreNumbersInPattern(patternSections, i + 1);
                    }
                    // if this character is asterisk and pattern contains more numbers, skip to next loop
                    if (moreNumbersInPattern && versionSections[i] == "*")
                    {
                        var resetValue = i >= (int) VersionComponent.Build ? buildAndRevisionResetValue : 0;
                        result.Add(resetValue.ToString());
                        i++;
                        break;
                    }
                    result.Add(versionSections[i]);
                }
                i++;
            }
            // extend version if ends with asterisk and pattern contains more numerical versions
            if (versionSections[i - 1] == "*" && moreNumbersInPattern)
            {
                var patternLength = patternSections.Length;
                while (i < patternLength)
                {
                    if (!MoreNumbersInPattern(patternSections, i))
                        break;
                    var resetValue = i >= (int) VersionComponent.Build ? buildAndRevisionResetValue : 0;
                    if (patternSections[i].StartsWith("+"))
                    {
                        var newValue = int.Parse(patternSections[i]) + resetValue;
                        result.Add(newValue.ToString());
                    }
                    else if (patternSections[i] == "*")
                    {
                        result.Add(resetValue.ToString());
                    }
                    else
                    {
                        result.Add(patternSections[i]);
                    }
                    i++;
                }
            }
            // setup the output string
            var sb = new StringBuilder();
            foreach (var s in result)
            {
                sb.Append(s);
                sb.Append(".");
            }
            return sb.ToString(0, sb.Length - 1);
        }


        /// <summary>
        ///     Class used for checking version formats.
        /// </summary>
        private class AssemblyVersionFormatProvider
        {
            private readonly AssemblyVersionType _assemblyVersionType;
            private readonly ProjectType _projectType;

            public AssemblyVersionFormatProvider(AssemblyVersionType assemblyVersionType, ProjectType projectType)
            {
                _assemblyVersionType = assemblyVersionType;
                _projectType = projectType;
            }

            public int MinLength
            {
                get
                {
                    switch (_assemblyVersionType)
                    {
                        case AssemblyVersionType.AssemblyVersion:
                            return 2;
                        case AssemblyVersionType.AssemblyFileVersion:
                            return 2;
                        case AssemblyVersionType.AssemblyInformationalVersion:
                            return 2;
                    }
                    return 2;
                }
            }

            public int MaxLength
            {
                get
                {
                    if (_projectType == ProjectType.SetupProject)
                        return 3;
                    return 4;
                }
            }

            public bool IsWildcardAllowed
            {
                get
                {
                    switch (_assemblyVersionType)
                    {
                        case AssemblyVersionType.AssemblyVersion:
                            return true;
                        case AssemblyVersionType.AssemblyFileVersion:
                            return false;
                        case AssemblyVersionType.AssemblyInformationalVersion:
                            return false;
                    }
                    return false;
                }
            }
        }

        /// <summary>
        ///     Validates version string and returns description string in the
        ///     case of invalid format. If version string is valid, returns empty
        ///     string.
        /// </summary>
        /// <param name="version">
        ///     Version string to validate.
        /// </param>
        /// <param name="assemblyVersionType">
        ///     <c>AssemblyVersionType</c> to which this string corresponds.
        /// </param>
        /// <param name="projectType">
        ///     <c>ProjectType</c> to which this string corresponds.
        /// </param>
        /// <returns>
        ///     String with error description or empty string if version is valid.
        /// </returns>
        private static string ValidateVersionString(string version, AssemblyVersionType assemblyVersionType, ProjectType projectType)
        {
            Debug.Assert(version != null);
            var avfp = new AssemblyVersionFormatProvider(assemblyVersionType, projectType);
            if (version.EndsWith("."))
                return TxtVersionMustNotEndWithDot;
            var versionParts = version.Split('.');
            if (versionParts.Length < avfp.MinLength)
                return string.Format(TxtVersionMustConsistOfAtLeastNComponents, avfp.MinLength);
            if (versionParts.Length > avfp.MaxLength)
                return string.Format(TxtVersionMustConsistOfAtMostNComponents, avfp.MaxLength);
            for (var i = 0; i < avfp.MinLength; i++)
                try
                {
                    var val = int.Parse(versionParts[i]);
                    if (val < 0)
                        return TxtMustNotContainNegativeIntegers;
                }
                catch
                {
                    return TxtMustBeNonNegativeIntegers;
                }
            // if there is asterisk in Build or Revision it must be the last component in version 
            if (avfp.IsWildcardAllowed)
                if (versionParts.Length > 3 && versionParts[2] == "*")
                    return TxtAsteriskMustBeLast;
            // flag controlling if number appears after an asterisk (asterisk 
            // must not be followed by integer version components)
            for (var i = avfp.MinLength; i < versionParts.Length; i++)
                if (versionParts[i] == "*")
                {
                    if (!avfp.IsWildcardAllowed)
                        return TxtNoAsteriskAllowed;
                }
                else
                {
                    int val;
                    try
                    {
                        val = int.Parse(versionParts[i]);
                    }
                    catch (FormatException)
                    {
                        return avfp.IsWildcardAllowed ? TxtMustBeNonNegativeIntegersOrAsterisk : TxtMustBeNonNegativeIntegers;
                    }
                    catch (OverflowException)
                    {
                        return TxtMustBeIntegerSmallerThanMaxValue;
                    }
                    if (val < 0)
                        return TxtMustNotContainNegativeIntegers;
                    if (val > MaxVersion)
                        return TxtMustBeIntegerSmallerThanMaxValue;
                }
            return string.Empty;
        }

        /// <summary>
        ///     Returns higher of two <c>ProjectVersion</c> objects provided.
        /// </summary>
        /// <param name="v1">
        ///     First <c>ProjectVersion</c> to compare.
        /// </param>
        /// <param name="v2">
        ///     Second <c>ProjectVersion</c> to compare.
        /// </param>
        /// <returns>
        ///     Reference two higher <c>ProjectVersion</c> objects.
        /// </returns>
        public static ProjectVersion Max(ProjectVersion v1, ProjectVersion v2)
        {
            var vc1 = v1.ComparableComponents;
            var vc2 = v2.ComparableComponents;
            for (var i = 0; i < 4; i++)
                if (vc1[i] != vc2[i])
                    if (vc1[i] != MaxVersion && vc2[i] != MaxVersion) return vc1[i] > vc2[i] ? v1 : v2;
                    else return vc1[i] != MaxVersion ? v1 : v2;
            return v1;
        }

        /// <summary>
        ///     Validates version string.
        /// </summary>
        /// <param name="version">
        ///     Version string to validate.
        /// </param>
        /// <param name="assemblyVersionType">
        ///     <c>AssemblyVersionType</c> to which this string corresponds.
        /// </param>
        /// <param name="projectType">
        ///     <c>ProjectType</c> to which this string corresponds.
        /// </param>
        /// <returns>
        ///     <c>true</c> if version is valid, else returns <c>false</c>.
        /// </returns>
        private static bool IsValidVersionString(string version, AssemblyVersionType assemblyVersionType, ProjectType projectType)
        {
            return ValidateVersionString(version, assemblyVersionType, projectType) == string.Empty;
        }

        /// <summary>
        ///     Checks if pattern to be applied is valid. Pattern must consist of
        ///     exactly four dot separated sections (Major, Minor, Build and
        ///     Revision). Each section may consist of an integer, an integer
        ///     preceeded by '+' character, '*' or '+' character.
        /// </summary>
        /// <param name="pattern">
        ///     Pattern to validate.
        /// </param>
        /// <returns>
        ///     <c>true</c> if pattern is a valid one, else returns <c>false</c>.
        /// </returns>
        public static bool IsValidPattern(string pattern)
        {
            var patternSections = pattern.Split('.');
            if (patternSections.Length != 4)
                return false;
            foreach (var section in patternSections)
                switch (section)
                {
                    case "*":
                    case "+":
                        break;
                    default:
                        try
                        {
                            var val = long.Parse(section);
                            if (val < 0 || val >= MaxVersion)
                                return false;
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                        break;
                }
            return true;
        }

        #endregion // Public static methods

        #region Private static methods

        private static string IncrementStringInteger(string integerValue, int increment)
        {
            if (integerValue == "*")
                return integerValue;
            // although Parse method may throw an exception, in this case 
            // integerValue should be a valid string representation of integer
            var n = long.Parse(integerValue);
            n += increment;
            if (n >= MaxVersion)
                throw new OverflowException();
            return n.ToString();
        }

        private static bool MoreNumbersInPattern(string[] sections, int index)
        {
            for (var i = index; i < sections.Length; i++) if (sections[i] != "*" && !sections[i].StartsWith("+")) return true;
            return false;
        }

        #endregion Private static methods

        #region Operators

        /// <summary>
        ///     Determines whether two specified instances of <c>ProjectVersion</c>
        ///     are equal.
        /// </summary>
        /// <param name="v1">
        ///     The first instance of <c>ProjectVersion</c>.
        /// </param>
        /// <param name="v2">
        ///     The second instance of <c>ProjectVersion</c>.
        /// </param>
        /// <returns>
        ///     <c>true</c> if v1 equals v2; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(ProjectVersion v1, ProjectVersion v2)
        {
            if ((object) v1 == null)
                return null == (object) v2;
            return v1.Equals(v2);
        }

        /// <summary>
        ///     Determines whether two specified instances of <c>ProjectVersion</c>
        ///     are not equal.
        /// </summary>
        /// <param name="v1">
        ///     The first instance of <c>ProjectVersion</c>.
        /// </param>
        /// <param name="v2">
        ///     The second instance of <c>ProjectVersion</c>.
        /// </param>
        /// <returns>
        ///     <c>true</c> if v1 does not equal v2; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(ProjectVersion v1, ProjectVersion v2)
        {
            return !(v1 == v2);
        }

        /// <summary>
        ///     Determines whether the first specified instance of <c>ProjectVersion</c>
        ///     is greater than the second specified instance of <c>ProjectVersion</c>.
        /// </summary>
        /// <param name="v1">
        ///     The first instance of <c>ProjectVersion</c>.
        /// </param>
        /// <param name="v2">
        ///     The second instance of <c>ProjectVersion</c>.
        /// </param>
        /// <returns>
        ///     <c>true</c> if v1 is greater than v2; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator >(ProjectVersion v1, ProjectVersion v2)
        {
            Debug.Assert(v1 != null);
            Debug.Assert(v2 != null);
            var vc1 = v1.ComparableComponents;
            var vc2 = v2.ComparableComponents;
            for (var i = 0; i < 4; i++)
                if (vc1[i] != vc2[i])
                    return vc1[i] > vc2[i];
            return false;
        }

        /// <summary>
        ///     Determines whether the first specified instance of <c>ProjectVersion</c>
        ///     is greater than or equal to the second specified instance of
        ///     <c>ProjectVersion</c>.
        /// </summary>
        /// <param name="v1">
        ///     The first instance of <c>ProjectVersion</c>.
        /// </param>
        /// <param name="v2">
        ///     The second instance of <c>ProjectVersion</c>.
        /// </param>
        /// <returns>
        ///     <c>true</c> if v1 is greater than or equal to v2;
        ///     otherwise, <c>false</c>.
        /// </returns>
        public static bool operator >=(ProjectVersion v1, ProjectVersion v2)
        {
            Debug.Assert(v1 != null);
            Debug.Assert(v2 != null);
            var vc1 = v1.ComparableComponents;
            var vc2 = v2.ComparableComponents;
            for (var i = 0; i < 4; i++)
                if (vc1[i] != vc2[i])
                    return vc1[i] >= vc2[i];
            return true;
        }

        /// <summary>
        ///     Determines whether the first specified instance of <c>ProjectVersion</c>
        ///     is less than the second specified instance of
        ///     <c>ProjectVersion</c>.
        /// </summary>
        /// <param name="v1">
        ///     The first instance of <c>ProjectVersion</c>.
        /// </param>
        /// <param name="v2">
        ///     The second instance of <c>ProjectVersion</c>.
        /// </param>
        /// <returns>
        ///     <c>true</c> if v1 is less than v2; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator <(ProjectVersion v1, ProjectVersion v2)
        {
            Debug.Assert(v1 != null);
            Debug.Assert(v2 != null);
            var vc1 = v1.ComparableComponents;
            var vc2 = v2.ComparableComponents;
            for (var i = 0; i < 4; i++)
                if (vc1[i] != vc2[i])
                    return vc1[i] < vc2[i];
            return false;
        }

        /// <summary>
        ///     Determines whether the first specified instance of <c>ProjectVersion</c>
        ///     is less than or equal to the second specified instance of
        ///     <c>ProjectVersion</c>.
        /// </summary>
        /// <param name="v1">
        ///     The first instance of <c>ProjectVersion</c>.
        /// </param>
        /// <param name="v2">
        ///     The second instance of <c>ProjectVersion</c>.
        /// </param>
        /// <returns>
        ///     <c>true</c> if v1 is less than or equal to v2;
        ///     otherwise, <c>false</c>.
        /// </returns>
        public static bool operator <=(ProjectVersion v1, ProjectVersion v2)
        {
            Debug.Assert(v1 != null);
            Debug.Assert(v2 != null);
            var vc1 = v1.ComparableComponents;
            var vc2 = v2.ComparableComponents;
            for (var i = 0; i < 4; i++)
                if (vc1[i] != vc2[i])
                    return vc1[i] <= vc2[i];
            return true;
        }

        #endregion // Operators
    }
}