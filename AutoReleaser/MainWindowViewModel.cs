using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using AutoReleaser.Builder;
using AutoReleaser.Datastore;
using AutoReleaser.SolutionLoader;
using AutoReleaser.SolutionLoader.Configuration;
using AutoReleaser.SolutionLoader.SolutionFileReaders;
using Mvvm;
using Mvvm.Commands;
using Ookii.Dialogs.Wpf;

namespace AutoReleaser
{
    public sealed class MainWindowViewModel : BindableBase
    {
        private bool _isBusy;
        private bool _unLocked;
        private bool _isSelected;

        public MainWindowViewModel()
        {
            Options = Store.StoreInstance.GetOptions();
            Options.PropertyChanged += Options_PropertyChanged;

            OpenFolderSolution = new DelegateCommand(OpenSolutionPath);
            RestartCommand = new DelegateCommand(Restart, CanRestart);
            SetupCommand = new DelegateCommand(Setup, CanCommonBuild);
            MinorCommand = new DelegateCommand(Minor, CanCommonBuild);
            MajorCommand = new DelegateCommand(Major, CanCommonBuild);

            UnLocked = true;
            Paragraph para = new Paragraph();
            Document = new FlowDocument(para);
            foreach (var releaseItem in Store.StoreInstance.GetReleaseItems())
                ReleaseItems.Add(releaseItem);
            CommonBuilder = new CommonBuilder(para, StartProcess, StopProcess, Options);

            SetProjects();
        }

        public Options Options { get; }

        public UISyncObservableCollection<ProjectFile> Projects { get; set; } = new UISyncObservableCollection<ProjectFile>();

        public DelegateCommand OpenFolderSolution { get; }
        
        private void Options_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Options.SolutionPath):
                    Task.Run(new Action(SetProjects));
                    break;
            }
        }

        private void OpenSolutionPath()
        {
            var path = SearchFolderPath();
            if (string.IsNullOrWhiteSpace(path)) return;

            Options.SolutionPath = path;
        }
        private string SearchFolderPath()
        {
            var diag = new VistaOpenFileDialog
            {
                Title = "Bitte Datei Whälen",
                Filter = ".sln|*.sln",
                CheckFileExists = true,
                Multiselect = false
            };

            return diag.ShowDialog(Application.Current.MainWindow) == true ? diag.FileName : null;
        }

        private void SetProjects()
        {
            var path = Options.SolutionPath;
            CommonBuilder.SetSolutionBrowser(null);
            Projects.Clear();

            if (!File.Exists(path)) return;

            var reader = SlnFileReader.SlnFileReaderFactory.GetSlnFileReader(path, ConfigurationPersister.InstanceField.Configuration);
            CommonBuilder.SetSolutionBrowser(reader);

            foreach (string name in reader.ProjectInfoList.ProjectInfos.Where(pi => pi.ProjectTypeInfo.ProjectType == ProjectType.CSharpProject).Select(pi => pi.ProjectName))
                Projects.Add(new ProjectFile(name, Options));
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }
        public bool UnLocked
        {
            get => _unLocked;
            set => SetProperty(ref _unLocked, value);
        }
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        private void StartProcess()
        {
            IsBusy = true;
            UnLocked = false;
        }
        private void StopProcess()
        {
            IsBusy = false;
            UnLocked = true;
            //IsSelected = true;
        }

        public ObservableCollection<ReleaseItem> ReleaseItems { get; } = new ObservableCollection<ReleaseItem>();
        public FlowDocument Document { get; }
        public CommonBuilder CommonBuilder { get; }

        public DelegateCommand RestartCommand { get; }
        public DelegateCommand SetupCommand { get; }
        public DelegateCommand MinorCommand { get; }
        public DelegateCommand MajorCommand { get; }

        private void Restart()
        {
            CommonBuilder.Restart(ReleaseItems.First(ri => !ri.Completed));
        }
        private bool CanRestart() => CommonBuilder.CanBuild() && ReleaseItems.Any(ri => !ri.Completed);

        private void Minor() => CommonBuilder.Build(UpdateType.Minor);
        private void Major() => CommonBuilder.Build(UpdateType.Major);
        private void Setup() => CommonBuilder.Setup();
        private bool CanCommonBuild() => CommonBuilder.CanBuild() && ReleaseItems.All(ri => ri.Completed);
    }
}