/*
 * Filename:    ConfigurationPersister.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Loads and saves configuration file. 
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
using AutoReleaser.SolutionLoader.Util;

namespace AutoReleaser.SolutionLoader.Configuration
{
    /// <summary>
    ///     Class responsible to load and save configuration file. Implemented
    ///     as a singleton to be accessible from entire code.
    /// </summary>
    public sealed class ConfigurationPersister
    {
        private const string Filename = "Configuration.xml";

        // ReSharper disable once InconsistentNaming
        private static readonly ConfigurationPersister _instanceField = new ConfigurationPersister();

        private readonly string _configurationFilename;

        /// <summary>
        ///     <c>XmlSerializer</c> object used to store/load configuration.
        /// </summary>
        //private XmlSerializer _xmlSerializer = null;
        // custom class is used becase <c>XmlSerializer</c> is throwing <c>NullReferenceException</c>s
        private readonly VcbConfigurationSerializer _xmlSerializer;

        /// <summary>
        ///     Configuration data object.
        /// </summary>
        private VcbConfiguration _configuration;

        private DateTime _lastConfigurationDateTime = DateTime.MinValue;

        /// <summary>
        ///     Creates <c>ConfigurationPersister</c> object. Tries to read
        ///     the configuration file. If reading fails, default configuration
        ///     is used.
        /// </summary>
        private ConfigurationPersister()
        {
            var configurationFolder = FileUtil.GetConfigurationFolder();
            _configurationFilename = Path.Combine(configurationFolder, Filename);
            // first create a default configuration
            _configuration = new VcbConfiguration();
            // custom class is used becase <c>XmlSerializer</c> is throwing <c>NullReferenceException</c>s
            //_xmlSerializer = new XmlSerializer(typeof(VcbConfiguration));
            _xmlSerializer = new VcbConfigurationSerializer();
            ReadConfiguration();
            // gets path to SourceSafe executable (required for command-line util)
            GetSourceSafePath();
            // get root directory of the IIS
            GetIisRoot();
        }

        /// <summary>
        ///     Accessor to <c>ConfigurationPesister</c> instance.
        /// </summary>
        public static ConfigurationPersister InstanceField
        {
            get
            {
                _instanceField.ReadConfiguration();
                return _instanceField;
            }
        }

        /// <summary>
        ///     Gets current configuration.
        /// </summary>
        public VcbConfiguration Configuration => _configuration;

        /// <summary>
        ///     Reads configuration file if it has been updated meanwhile.
        /// </summary>
        private void ReadConfiguration()
        {
            Debug.Assert(_xmlSerializer != null);
            if (File.Exists(_configurationFilename))
            {
                var lastWrite = File.GetLastWriteTime(_configurationFilename);
                if (lastWrite > _lastConfigurationDateTime)
                    try
                    {
                        _configuration = _xmlSerializer.Deserialize(_configurationFilename);
                        _configuration.ConfigurationFileRead = true;
                        _lastConfigurationDateTime = File.GetLastWriteTime(_configurationFilename);
                    }
                    catch (Exception exception)
                    {
                        Trace.WriteLine(exception.ToString());
                    }
            }
            Debug.Assert(_configuration != null);
        }

        /// <summary>
        ///     Gets the IIS root, reading it from registry.
        /// </summary>
        private void GetIisRoot()
        {
            Debug.Assert(_configuration?.FoldersConfigurations != null);
            var iis = _configuration.FoldersConfigurations.IisFolder;
            Debug.Assert(iis != null);
            // if no folder set but is available (e.g. when starting new version
            // with old configuration)
            if (!iis.IsAvailable || iis.Folder.Length != 0) return;
            if (InetRootLocator.Instance.IsIisAvailable)
                iis.Folder = InetRootLocator.Instance.PathWwwRoot;
            else
                iis.IsAvailable = false;
        }

        /// <summary>
        ///     Gets the SourceSafe executable path, reading from registry.
        /// </summary>
        private void GetSourceSafePath()
        {
            Debug.Assert(_configuration?.FoldersConfigurations != null);
            var ss = _configuration.FoldersConfigurations.SourceSafeFolder;
            Debug.Assert(ss != null);
            // if no folder set but is available (e.g. when starting new version
            // with old configuration)
            if (ss.IsAvailable && ss.Folder.Length == 0)
                ss.IsAvailable = false;
        }
    }
}