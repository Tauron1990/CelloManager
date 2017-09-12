using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Security.Principal;
using System.Threading;
using JetBrains.Annotations;
using Tauron.Application.CelloManager.Properties;
using Tauron.Application.Common.Updater;
using Tauron.Application.Common.Updater.Provider;
using Tauron.Application.Common.Updater.Service;
using Tauron.Application.Implement;

namespace Tauron.Application.CelloManager
{
    public static class Programm
    {
        [STAThread]
        [LoaderOptimization(LoaderOptimization.SingleDomain)]
        public static void Main()
        {
            var applicationIdentifier = "CelloManager";
            if (SecurityHelper.IsEnvironmentPermissionGranted())
                applicationIdentifier += Environment.UserName;

            UpdaterService.SetGithubProvider("Tauron1990", "CelloManager", Tuple.Create("Cello_Manager", string.Empty), VersionExtractor);
            var configuration = UpdaterService.Configuration;
            configuration.CurrentVersion = Assembly.GetEntryAssembly().GetName().Version;
            configuration.FileSelector += FileSelector;
            configuration.SetupFile = "CelloManager.exe";
            configuration.StartFile = "CelloManager.exe";

            var stade = UpdaterService.UpdateManager.Setup();

            switch (stade)
            {
                case InstallerStade.Shudown:
                    return;
            }

            try
            {
                var channelName = string.Concat(applicationIdentifier, ":", "SingeInstanceIPCChannel");
                var mutex = new Mutex(true, applicationIdentifier, out var first);

                try
                {
                    if (!first)
                        SignalFirstInstance(channelName, applicationIdentifier);

                    var domain = AppDomain.CurrentDomain;
                    domain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
                    domain.UnhandledException += OnUnhandledException;

                    //AppDomain.CurrentDomain.SetData("DataDirectory", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).CombinePath("Tauron\\CelloManager"));

                    StartApp(mutex, channelName);
                }
                finally
                {
                    CleanUp();
                }
            }
            catch (MethodAccessException)
            {
                CultureInfo.CurrentCulture = Settings.Default.Language;
                WpfApplication.Run<App>(null, Settings.Default.Language);
            }
        }

        private static ReleaseFile FileSelector(Release release)
        {
            return release.Files.First(file => file.Name == "Release.zip");
        }
        private static Version VersionExtractor(string s)
        {
            s = s.Trim('v');

            if (Version.TryParse(s, out var version)) return version;

            return null;
        }


        private static void SignalFirstInstance([NotNull] string channelName, [NotNull] string applicationIdentifier)
        {
            if (channelName == null) throw new ArgumentNullException(nameof(channelName));
            if (applicationIdentifier == null) throw new ArgumentNullException(nameof(applicationIdentifier));

            SingleInstance<App>.SignalFirstInstance(channelName,
                SingleInstance<App>.GetCommandLineArgs(applicationIdentifier));
        }

        private static void CleanUp()
        {
            SingleInstance<App>.Cleanup();
        }


        private static void StartApp([NotNull] Mutex mutex, [NotNull] string channelName)
        {
            App.Setup(mutex, channelName);
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        private static void OnUnhandledException([NotNull] object sender, [NotNull] UnhandledExceptionEventArgs args)
        {
            CommonConstants.LogCommon(true, args.ExceptionObject.ToString());
        }
    }
}