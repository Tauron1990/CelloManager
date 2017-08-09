using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Security.Principal;
using System.Threading;
using JetBrains.Annotations;
using Tauron.Application.CelloManager.Properties;
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


            try
            {
                var channelName = string.Concat(applicationIdentifier, ":", "SingeInstanceIPCChannel");
                bool first;
                var mutex = new Mutex(true, applicationIdentifier, out first);

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