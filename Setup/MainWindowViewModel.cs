using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Ookii.Dialogs.Wpf;
using Syncfusion.Windows.Tools.Controls;
using Tauron.Application.CelloManager.Setup.Resources;
using Tauron.Application.Common.Updater;
using File = System.IO.File;

namespace Tauron.Application.CelloManager.Setup
{
    public class MainWindowViewModel
    {
        private Window _window;
        private WizardControl _wizardControl;
        private Dispatcher _dispatcher;

        public ResourceWrapper ResourceWrapper { get; } = new ResourceWrapper();
        public PageContext PageContext { get; } = new PageContext();

        public void SetWindow(Window window, WizardControl control)
        {
            _window = window;
            _wizardControl = control;
            _dispatcher = _window.Dispatcher;
        }

        public void OnCancel()
        {
            _window.Close();
        }

        public void OnFinish()
        {
            string path = Path.Combine(PageContext.InstallLocation, "CelloManager.exe");
            //try
            //{
            //    if (File.Exists(path) && PageContext.CreateShortcut)
            //    {
            //    }
            //}
            //catch(Exception e) when(e is COMException || e is Win32Exception || e is UnauthorizedAccessException)
            //{
            //}

            if (PageContext.StartApp == true && File.Exists(path))
                Process.Start(path);

            _window.Close();
        }

        public void OnSelectedPageChanged()
        {
            if ((string) _wizardControl.SelectedWizardPage.Tag == "Progress")
            {
                Task.Run(new Action(Install));
            }
        }

        private void Install()
        {
            string temp = Path.Combine(Path.GetTempPath(), "TauronInstall");
            if(Directory.Exists(temp))
                Directory.Delete(temp, true);
            Directory.CreateDirectory(temp);

            UpdaterService.SetGithubProvider("Tauron1990", "CelloManager", Tuple.Create("Cello_Manager_Installer", "1.0"), s =>
            {
                s = s.Trim('v');

                if (Version.TryParse(s, out var version)) return version;

                return null;
            }, temp);

            UpdaterService.Configuration.SetupCleanUp = () => Directory.Delete(temp, true);
            UpdaterService.Configuration.FileSelector = release => release.Files.First(file => file.Name == "Release.zip");
            UpdaterService.Configuration.Provider.Downloader.ProgressEvent += (o, args) => PageContext.ProgressPercent = args.Percent;
            UpdaterService.Configuration.Provider.Preperator.PreperationInProgressEvent += (o, args) => PageContext.ProgressPercent = args.Percent;

            PageContext.InstallMessege = UIResources.InstallerMessageDownloading;
            PageContext.IsIndeterminate = false;
            PageContext.ProgressPercent = 0;

            string downloadLocation = UpdaterService.InstallManager.DownloadUpdate(UpdaterService.UpdateManager.GetLasted());

            PageContext.ProgressPercent = 0;
            PageContext.InstallMessege = UIResources.InstallerMessageCopy;

            if (!Directory.Exists(PageContext.InstallLocation))
                Directory.CreateDirectory(PageContext.InstallLocation);
            UpdaterService.InstallManager.ExecuteSetup(PageContext.InstallLocation, downloadLocation, new Version(0,0));

            PageContext.ProgressPercent = 0;
            PageContext.InstallMessege = UIResources.PrograssInitialMessage;
            PageContext.IsIndeterminate = true;

            _dispatcher.BeginInvoke(new Action(() => _wizardControl.MoveNext()));
        }

        public void OnBrowseClick()
        {
            VistaFolderBrowserDialog diag = new VistaFolderBrowserDialog
            {
                Description = UIResources.WinzadFolderBrowserDialogDescription,
                RootFolder = Environment.SpecialFolder.ProgramFiles,
                ShowNewFolderButton = true
            };

            if (diag.ShowDialog(_window) == true)
                PageContext.InstallLocation = diag.SelectedPath;
        }
    }
}