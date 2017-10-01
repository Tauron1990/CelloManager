/*
 * Filename:    AssemblyVersions.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Wrapper arround the set of versions (AssemblyVersion, 
 *              AssemblyFileVersion and AssemblyInformationalVersion).
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AutoReleaser.SolutionLoader.Configuration;

namespace AutoReleaser.SolutionLoader.Versions
{
    /// <summary>
    ///     Class encapsulating AssemblyVersion, AssemblyFileVersion and
    ///     AssemblyInformationalVersion.
    /// </summary>
    public class AssemblyVersions : ICloneable
    {
        private readonly Dictionary<AssemblyVersionType, ProjectVersion> _versions;

        #region Constructors

        private AssemblyVersions() : this(ProjectVersion.Empty, ProjectVersion.Empty, ProjectVersion.Empty)
        {
        }

        private AssemblyVersions(AssemblyVersions assemblyVersions)
            : this(assemblyVersions[AssemblyVersionType.AssemblyVersion].Clone(),
                assemblyVersions[AssemblyVersionType.AssemblyFileVersion].Clone(),
                assemblyVersions[AssemblyVersionType.AssemblyInformationalVersion].Clone())
        {
        }

        public AssemblyVersions(ProjectVersion assemblyVersion, ProjectVersion assemblyFileVersion, ProjectVersion assemblyInformationalVersion)
        {
            _versions = new Dictionary<AssemblyVersionType, ProjectVersion>
            {
                [AssemblyVersionType.AssemblyVersion] = assemblyVersion,
                [AssemblyVersionType.AssemblyFileVersion] = assemblyFileVersion,
                [AssemblyVersionType.AssemblyInformationalVersion] = assemblyInformationalVersion
            };
        }

        #endregion // Constructors

        #region Public properties

        public ProjectVersion HighestProjectVersion
        {
            get
            {
                var highest = ProjectVersion.MinValue;
                foreach (var pv in _versions.Values.Where(pv => highest < pv))
                    highest = pv;
                return highest.Clone();
            }
        }

        public bool AreVersionsSynchronized
        {
            get
            {
                var validVersionTypes = GetValidVersionTypes();
                if (validVersionTypes.Length < 1)
                    return true;
                var reference = _versions[validVersionTypes[0]];
                for (var i = 1; i < validVersionTypes.Length; i++)
                    if (reference != _versions[validVersionTypes[i]])
                        return false;
                return true;
            }
        }

        public AssemblyVersionType[] GetValidVersionTypes()
        {
            return AssemblyVersionTypes.Where(avt => this[avt] != ProjectVersion.Empty).ToArray();
        }

        public ProjectVersion this[AssemblyVersionType type] => _versions[type];

        #endregion //Public properties

        #region Public methods

        public void SynchronizeVersionsToHighest()
        {
            var highestProjectVersion = HighestProjectVersion;
            if (highestProjectVersion == ProjectVersion.MinValue) return;

            if (_versions[AssemblyVersionType.AssemblyVersion] != ProjectVersion.Empty)
                _versions[AssemblyVersionType.AssemblyVersion] = highestProjectVersion;
            if (_versions[AssemblyVersionType.AssemblyFileVersion] != ProjectVersion.Empty)
                _versions[AssemblyVersionType.AssemblyFileVersion] = highestProjectVersion;
            if (_versions[AssemblyVersionType.AssemblyInformationalVersion] != ProjectVersion.Empty)
                _versions[AssemblyVersionType.AssemblyInformationalVersion] = highestProjectVersion;
        }

        public bool ContainsVersion(AssemblyVersionType assemblyVersionType)
        {
            Debug.Assert(assemblyVersionType != AssemblyVersionType.All && assemblyVersionType != AssemblyVersionType.None);
            return Array.IndexOf(GetValidVersionTypes(), assemblyVersionType) > -1;
        }

        #endregion // Public methods

        #region ICloneable implementation

        object ICloneable.Clone()
        {
            return Clone();
        }

        private AssemblyVersions Clone()
        {
            return new AssemblyVersions(this);
        }

        #endregion // ICloneable implementation

        #region Static methods

        public static AssemblyVersions Max(AssemblyVersions assemblyVersions1, AssemblyVersions assemblyVersions2)
        {
            var assemblyVersion = ProjectVersion.Max(assemblyVersions1[AssemblyVersionType.AssemblyVersion], assemblyVersions2[AssemblyVersionType.AssemblyVersion]).Clone();
            var assemblyFileVersion = ProjectVersion.Max(assemblyVersions1[AssemblyVersionType.AssemblyFileVersion], assemblyVersions2[AssemblyVersionType.AssemblyFileVersion]).Clone();
            var assemblyInformationalVersion = ProjectVersion.Max(assemblyVersions1[AssemblyVersionType.AssemblyInformationalVersion], assemblyVersions2[AssemblyVersionType.AssemblyInformationalVersion]).Clone();
            return new AssemblyVersions(assemblyVersion, assemblyFileVersion, assemblyInformationalVersion);
        }

        public static AssemblyVersions Max(AssemblyVersions assemblyVersions, ProjectInfo projectInfo)
        {
            return Max(assemblyVersions, projectInfo.ToBecomeAssemblyVersions);
        }

        public static AssemblyVersions MaxProposed(AssemblyVersions assemblyVersions, ProjectInfo projectInfo)
        {
            if (projectInfo.Modified)
                return Max(assemblyVersions, projectInfo.ToBecomeAssemblyVersions);
            return Max(assemblyVersions, projectInfo.CurrentAssemblyVersions);
        }

        #endregion // Static methods

        #region Static fields

        public static readonly AssemblyVersions Empty = new AssemblyVersions();

        public static readonly AssemblyVersions MinValue = new AssemblyVersions(ProjectVersion.MinValue, ProjectVersion.MinValue, ProjectVersion.MinValue);

        public static readonly AssemblyVersionType[] AssemblyVersionTypes = {AssemblyVersionType.AssemblyVersion, AssemblyVersionType.AssemblyFileVersion, AssemblyVersionType.AssemblyInformationalVersion};

        #endregion // Static fields
    }
}