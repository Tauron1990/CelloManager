using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JetBrains.Annotations;
using Syncfusion.Licensing;
using Tauron.Application.CelloManager.Data;
using Tauron.Application.CelloManager.Logic;
using Tauron.Application.CelloManager.Properties;
using Tauron.Application.CelloManager.Resources;
using Tauron.Application.CelloManager.UI;
using Tauron.Application.Common.BaseLayer.BusinessLayer;
using Tauron.Application.Implement;
using Tauron.Application.Implementation;
using Tauron.Application.Ioc;
using Tauron.Application.Views;

namespace Tauron.Application.CelloManager
{
    internal class App : WpfApplication, ISingleInstanceApp
    {
        public App()
            : base(true)
        {
            SyncfusionLicenseProvider.RegisterLicense("OTg2NUAzMTM2MmUzMjJlMzBJSVNzUDVuTU82QVhhbXZLOVNFclYwd0dJWm80QXdjQTRSL0FGY0FNOHhRPQ==");
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            MainWindow?.Focus();
            return true;
        }

        public static void Setup([NotNull] Mutex mutex, [NotNull] string channelName)
        {
            if (mutex == null) throw new ArgumentNullException(nameof(mutex));
            if (channelName == null) throw new ArgumentNullException(nameof(channelName));

            Run<App>(app => SingleInstance<App>.InitializeAsFirstInstance(mutex, channelName, app), Settings.Default.Language);
        }

        protected override void ConfigSplash()
        {
            var dic = new PackUriHelper().Load<ResourceDictionary>("StartResources.xaml");

            CurrentWpfApplication.Resources = dic;

            var control = new ContentControl
            {
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Height = 236,
                Width = 414,
                Content = dic["MainLabel"]
            };

            SplashMessageListener.CurrentListner.SplashContent = control;
            SplashMessageListener.CurrentListner.MainLabelForeground = "Black";
            SplashMessageListener.CurrentListner.MainLabelBackground = dic["MainLabelbackground"];
        }

        protected override IWindow DoStartup(CommandLineProcessor prcessor)
        {
            var temp = ViewManager.Manager.CreateWindow(AppConststands.MainWindowName);
            MainWindow = temp;

            CurrentWpfApplication.Dispatcher.Invoke(() =>
            {
                Current.MainWindow = temp;
                CurrentWpfApplication.MainWindow = (Window) temp.TranslateForTechnology();
            });
            return temp;
        }

        protected override void LoadCommands()
        {
            base.LoadCommands();
            CommandBinder.Register(ApplicationCommands.Close);
            CommandBinder.AutoRegister = true;
        }

        protected override void LoadResources()
        {
            SimpleLocalize.Register(UIResources.ResourceManager, typeof(UIModule).Assembly);
            //SimpleLocalize.Register(UILabels.ResourceManager, typeof(UpdaterService).Assembly);

            //System.Windows.Application.Current.Resources.MergedDictionaries.Add(
            //                                                                    (ResourceDictionary)
            //                                                                    System.Windows.Application.LoadComponent(new PackUriHelper()
            //                                                                                                                 .GetUri("Theme.xaml", typeof(App).Assembly.FullName, false)));
        }

        public override string GetdefaultFileLocation()
        {
            return GetDicPath();
        }

        protected override void MainWindowClosed(object sender, EventArgs e)
        {
            base.MainWindowClosed(sender, e);

            OnExit();
        }

        private static string GetDicPath()
        {
            return
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                    .CombinePath("Tauron\\CelloManager");
        }

        protected override void Fill(IContainer container)
        {
            ExportResolver resolver = new ExportResolver();
            resolver.AddAssembly(typeof(RuleFactory).Assembly);
            resolver.AddAssembly(typeof(App).Assembly);
            resolver.AddAssembly(typeof(WpfApplication).Assembly);
            resolver.AddAssembly(typeof(CommonApplication).Assembly);
            resolver.AddAssembly(typeof(DialogFactory).Assembly);
            //resolver.AddAssembly(typeof(UpdaterService).Assembly);

            resolver.AddAssembly(typeof(AppConststands).Assembly);
            resolver.AddAssembly(typeof(DataModule).Assembly);
            resolver.AddAssembly(typeof(LogicModule).Assembly);
            resolver.AddAssembly(typeof(UIModule).Assembly);
            resolver.AddAssembly(typeof(UIResources).Assembly);

            container.Register(resolver);
        }
    }
}