using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AutoReleaser.Builder;
using JetBrains.Annotations;

namespace AutoReleaser.Datastore
{
    [Serializable]
    public sealed class ReleaseItem : INotifyPropertyChanged
    {
        private UpdateType _updateType;
        private bool _test;
        private bool _version;
        private bool _build;
        private bool _upload;

        [UsedImplicitly]
        public int Id { get; set; }

        [UsedImplicitly]
        public DateTime InitialTime { get; set; }

        public bool Completed => Test && Version && Build && Upload;

        public bool Test
        {
            get => _test;
            set
            {
                if(_test == value) return;

                _test = value;
                Update();
                OnPropertyChanged();
            }
        }

        public bool Version
        {
            get => _version;
            set
            {
                if(_version == value) return;
                _version = value;
                Update();
                OnPropertyChanged();
            }
        }

        public bool Build
        {
            get => _build;
            set
            {
                if(_build == value) return;
                _build = value;
                Update();
                OnPropertyChanged();
            }
        }

        public bool Upload
        {
            get => _upload;
            set
            {
                if(_upload == value) return;
                _upload = value;
                Update();
                OnPropertyChanged();
            }
        }

        public UpdateType UpdateType
        {
            get => _updateType;
            set
            {
                if(_updateType == value) return;

                _updateType = value;

                Update();
                OnPropertyChanged();
            }
        }

        public string FullPath { get; }

        public ReleaseItem(string fullPath)
        {
            FullPath = fullPath;
            InitialTime = DateTime.Now;
        }

        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Update()
        {
            Store.StoreInstance.SaveContainer();
        }
    }
}