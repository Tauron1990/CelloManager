using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using LiteDB;

namespace AutoReleaser.Datastore
{
    [Serializable]
    public sealed class Options : INotifyPropertyChanged
    {
        private string _gitHubName;
        private string _gitHubRepository;
        private string _solutionPath;
        private string _setupProject;
        private string _setupBootstrapper;

        //[UsedImplicitly]
        //[BsonId]
        //public int Id { get; set; }

        public string GitHubName
        {
            get => _gitHubName;
            set
            {
                if(_gitHubName == value) return;

                _gitHubName = value;
                Update();
                OnPropertyChanged();
            }
        }

        public string GitHubRepository
        {
            get => _gitHubRepository;
            set
            {
                if(_gitHubRepository == value) return;

                _gitHubRepository = value;
                Update();
                OnPropertyChanged();
            }
        }

        public string SolutionPath
        {
            get => _solutionPath;
            set
            {
                if(_solutionPath ==  value) return;

                _solutionPath = value;
                Update();
                OnPropertyChanged();
            }
        }

        public string SetupProject
        {
            get => _setupProject;
            set
            {
                if(_setupProject == value) return;

                _setupProject = value;
                Update();
                OnPropertyChanged();
            }
        }

        public string SetupBootstrapper
        {
            get => _setupBootstrapper;
            set
            {
                if(_setupBootstrapper == value) return;

                _setupBootstrapper = value;
                Update();
                OnPropertyChanged();
            }
        }

        [UsedImplicitly]
        public Dictionary<string, bool> Tests { get; set; } = new Dictionary<string, bool>();

        [UsedImplicitly]
        public Dictionary<string, bool> Application { get; set; } = new Dictionary<string, bool>();

        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public void Update()
        {
            Store.StoreInstance.UpdateOptions(this);
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}