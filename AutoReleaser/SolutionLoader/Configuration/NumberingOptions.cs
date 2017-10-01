/*
 * Filename:    NumberingOptions.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Options used for the next version.
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
    [Flags]
    public enum AssemblyVersionType
    {
        None = 0,
        AssemblyVersion = 1,
        AssemblyFileVersion = 2,
        AssemblyInformationalVersion = 4,
        All = AssemblyVersion | AssemblyFileVersion | AssemblyInformationalVersion
    }

    [Serializable]
    public enum IncrementScheme
    {
        IncrementMajorVersion,
        IncrementMinorVersion,
        IncrementBuild,
        IncrementRevision
    }

    [Serializable]
    public enum BatchCommandIncrementScheme
    {
        IncrementModifiedIndependently,
        IncrementAllIndependently,
        IncrementModifiedOnlyAndSynchronize,
        IncrementAllAndSynchronize
    }

    [Serializable]
    [Flags]
    public enum ResetBuildAndRevision
    {
        ToZero = 0
    }

    [Serializable]
    public class NumberingOptions : ICloneable
    {
        private bool _allowArbitraryInformationalVersion;
        private bool _applyToAllTypes;
        private bool _autoBuildAndRevisionNumbering;

        private BatchCommandIncrementScheme _batchCommandIncrementScheme;
        private AssemblyVersionType _defaultVersionType;
        private bool _generatePackageAndProductCode;
        private bool _includeSetupProjects;
        private bool _includeVCppResourceFiles;
        private int _incrementBy;

        private IncrementScheme _incrementScheme;
        private bool _replaceAsteriskWithVersionComponents;
        private ResetBuildAndRevision _resetBuildAndRevisionTo;
        private bool _resetBuildOnMajorIncrement;
        private bool _resetBuildOnMinorIncrement;
        private bool _resetRevisionOnBuildIncrement;
        private bool _resetRevisionOnMajorIncrement;
        private bool _resetRevisionOnMinorIncrement;

        private bool _saveFilesBeforeRunningAddinCommand;
        private bool _synchronizeAllVersionTypes;

        public NumberingOptions()
        {
            _saveFilesBeforeRunningAddinCommand = true;
            _defaultVersionType = AssemblyVersionType.AssemblyVersion;
            _applyToAllTypes = false;
            _synchronizeAllVersionTypes = false;
            _allowArbitraryInformationalVersion = true;
            _includeVCppResourceFiles = true;
            _includeSetupProjects = true;
            _generatePackageAndProductCode = false;

            _autoBuildAndRevisionNumbering = false;
            _incrementScheme = IncrementScheme.IncrementRevision;
            _incrementBy = 1;
            _batchCommandIncrementScheme = BatchCommandIncrementScheme.IncrementModifiedIndependently;
            _resetBuildOnMajorIncrement = true;
            _resetBuildOnMinorIncrement = true;
            _resetRevisionOnMajorIncrement = true;
            _resetRevisionOnMinorIncrement = true;
            _resetRevisionOnBuildIncrement = true;
            _resetBuildAndRevisionTo = ResetBuildAndRevision.ToZero;
            _replaceAsteriskWithVersionComponents = false;
        }

        public bool SaveModifiedFilesBeforeRunningAddinCommand
        {
            get => _saveFilesBeforeRunningAddinCommand;
            set => _saveFilesBeforeRunningAddinCommand = value;
        }

        public AssemblyVersionType DefaultVersionType
        {
            get => _defaultVersionType;
            set => _defaultVersionType = value;
        }

        public int IncrementBy
        {
            get => _incrementBy;
            set => _incrementBy = value;
        }

        public bool AllowArbitraryInformationalVersion
        {
            get => _allowArbitraryInformationalVersion;
            set => _allowArbitraryInformationalVersion = value;
        }

        public bool IncludeVCppResourceFiles
        {
            get => _includeVCppResourceFiles;
            set => _includeVCppResourceFiles = value;
        }

        public bool IncludeSetupProjects
        {
            get => _includeSetupProjects;
            set => _includeSetupProjects = value;
        }

        public bool GeneratePackageAndProductCodes
        {
            get => _generatePackageAndProductCode;
            set => _generatePackageAndProductCode = value;
        }

        public bool ApplyToAllTypes
        {
            get => _applyToAllTypes;
            set => _applyToAllTypes = value;
        }

        public bool SynchronizeAllVersionTypes
        {
            get => _synchronizeAllVersionTypes;
            set => _synchronizeAllVersionTypes = value;
        }

        public IncrementScheme IncrementScheme
        {
            get => _incrementScheme;
            set => _incrementScheme = value;
        }

        public BatchCommandIncrementScheme BatchCommandIncrementScheme
        {
            get => _batchCommandIncrementScheme;
            set => _batchCommandIncrementScheme = value;
        }

        public bool UseDateTimeBasedBuildAndRevisionNumbering
        {
            get => _autoBuildAndRevisionNumbering;
            set => _autoBuildAndRevisionNumbering = value;
        }

        public bool ResetBuildOnMajorIncrement
        {
            get => _resetBuildOnMajorIncrement;
            set => _resetBuildOnMajorIncrement = value;
        }

        public bool ResetBuildOnMinorIncrement
        {
            get => _resetBuildOnMinorIncrement;
            set => _resetBuildOnMinorIncrement = value;
        }

        public bool ResetRevisionOnMajorIncrement
        {
            get => _resetRevisionOnMajorIncrement;
            set => _resetRevisionOnMajorIncrement = value;
        }

        public bool ResetRevisionOnMinorIncrement
        {
            get => _resetRevisionOnMinorIncrement;
            set => _resetRevisionOnMinorIncrement = value;
        }

        public bool ResetRevisionOnBuildIncrement
        {
            get => _resetRevisionOnBuildIncrement;
            set => _resetRevisionOnBuildIncrement = value;
        }

        public ResetBuildAndRevision ResetBuildAndRevisionTo
        {
            get => _resetBuildAndRevisionTo;
            set => _resetBuildAndRevisionTo = value;
        }

        public bool ReplaceAsteriskWithVersionComponents
        {
            get => _replaceAsteriskWithVersionComponents;
            set => _replaceAsteriskWithVersionComponents = value;
        }

        #region ICloneable implementation

        object ICloneable.Clone()
        {
            return Clone();
        }

        public NumberingOptions Clone()
        {
            return (NumberingOptions) MemberwiseClone();
        }

        #endregion // ICloneable implementation
    }
}