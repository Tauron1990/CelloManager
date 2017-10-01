using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AutoReleaser.Builder;
using JetBrains.Annotations;
using LiteDB;

namespace AutoReleaser.Datastore
{
    public sealed class ReleaseItem : INotifyPropertyChanged
    {
        private bool _completed;
        private UpdateType _updateType;

        [UsedImplicitly]
        public int Id { get; set; }

        [UsedImplicitly]
        public DateTime InitialTime { get; set; }

        public bool Completed
        {
            get => _completed;
            set
            {
                if(_completed == value) return;

                _completed = value;
                OnPropertyChanged();
                Update();
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

        public ReleaseItem()
        {
            InitialTime = DateTime.Now;
        }

        [field:BsonIgnore]
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Update()
        {
            Store.StoreInstance.UpdateReleaseItem(this);
        }
    }
}