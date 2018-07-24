using System;
using Tauron.Application.CelloManager.Logic.Historie;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews.OrderView
{
    public class OrderedSpool : ObservableObject
    {
        public CommittedSpool Spool { get; }
        private readonly Action _chnagedHandler;
        private bool _isChecked;

        public OrderedSpool(CommittedSpool spool, Action chnagedHandler)
        {
            Spool = spool;
            _chnagedHandler = chnagedHandler;
            Label  = $"{Spool.Name} - {Spool.Type}:";
        }

        public string Label { get; }

        public int OrderedCount
        {
            get => Spool.OrderedCount;
            set
            {
                Spool.OrderedCount = value;
                OnPropertyChanged();
            }
        }

        public bool IsChecked
        {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value, _chnagedHandler);
        }
    }
}