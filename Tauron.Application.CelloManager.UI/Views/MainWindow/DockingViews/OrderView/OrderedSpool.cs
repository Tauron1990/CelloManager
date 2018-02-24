using System;
using Tauron.Application.CelloManager.Logic.Historie;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews.OrderView
{
    public class OrderedSpool : ObservableObject
    {
        private readonly CommittedSpool _spool;
        private readonly Action _chnagedHandler;
        private bool _isChecked;

        public OrderedSpool(CommittedSpool spool, Action chnagedHandler)
        {
            _spool = spool;
            _chnagedHandler = chnagedHandler;
            Label  = $"{_spool.Name} - {_spool.Type}:";
        }

        public string Label { get; }

        public int OrderedCount
        {
            get => _spool.OrderedCount;
            set
            {
                _spool.OrderedCount = value;
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