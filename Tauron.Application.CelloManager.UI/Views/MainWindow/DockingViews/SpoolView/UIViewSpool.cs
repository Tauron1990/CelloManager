using System;
using System.ComponentModel;
using System.Windows.Input;
using Tauron.Application.CelloManager.Logic.Manager;
using Tauron.Application.CelloManager.Resources;
using Tauron.Application.CelloManager.UI.Models;
using Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews.Helper;
using Tauron.Application.Commands;
using Tauron.Application.Models;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews
{
    //[ExportViewModel(AppConststands.SingleUISpoolViwName)]
    public class UIViewSpool : ViewModelBase, IEquatable<UIViewSpool>
    {
        private readonly ISpoolManager _manager;
        private readonly SpoolModel _model;
        private readonly CelloSpool _spool;
        private int _stepCount;
        private bool _isUpdating;

        public UIViewSpool(CelloSpool spool, ISpoolManager manager, SpoolModel model)
        {
            _spool = spool;
            _manager = manager;
            _model = model;

            StepCount = 1;
            
            _spool.PropertyChanged += SpoolOnPropertyChanged;

            AddCommand = new SimpleCommand(Add);
            RemoveCommand = new SimpleCommand(Remove);
        }

        public UIViewSpool()
            : this(new CelloSpool("Test", "64", 100, 200, -1), new UIDummySpoolManager(), null)
        {
        }

        public string FirstTwo => _spool.Name.Substring(0, 2);
        public string LastText => _spool.Name.Substring(2, _spool.Name.Length - 2);

        public string AmountText => _spool.Amount + " " + UIResources.CommonLabelOf + " " + _spool.Neededamount;

        public CelloSpool Spool { get; }

        public int StepCount
        {
            get => _stepCount;
            set
            {
                _stepCount = value;
                OnPropertyChanged();
            }
        }
        
        public override string ToString()
        {
            return $"Name: {_spool.Name} + Type: {_spool.Type}";
        }

        private void SpoolOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            OnPropertyChanged(propertyChangedEventArgs);
            if (propertyChangedEventArgs.PropertyName == nameof(_spool.Name))
            {
                OnPropertyChangedExplicit(nameof(FirstTwo));
                OnPropertyChangedExplicit(nameof(LastText));
            }

            if (propertyChangedEventArgs.PropertyName == nameof(_spool.Amount) || propertyChangedEventArgs.PropertyName == nameof(_spool.Neededamount))
                OnPropertyChangedExplicit(nameof(AmountText));
        }

        public ICommand AddCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }

        public void Add(object var)
        {
            var count = GetStepCount();

            if (_manager.AddSpoolAmount(_spool, count))
                _spool.Amount += count;
        }
        
        public void Remove(object var)
        {
            var count = GetStepCount();

            if (_manager.SpoolEmpty(_spool, count))
                _spool.Amount -= count;

            InvalidateRequerySuggested();
        }

        private int GetStepCount()
        {
            var temp = StepCount;

            StepCount = 1;

            return temp;
        }

        public void Deattach() => _spool.PropertyChanged -= SpoolOnPropertyChanged;

        #region Equals

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            return obj.GetType() == GetType() && Equals((UIViewSpool) obj);
        }

        public bool Equals(UIViewSpool other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || Equals(_spool, other._spool);
        }

        public override int GetHashCode()
        {
            return _spool?.GetHashCode() ?? 0;
        }

        public static bool operator ==(UIViewSpool left, UIViewSpool right)
        {
            return Equals(left, right);
        }

        public static bool operator ==(CelloSpool left, UIViewSpool right)
        {
            return right != null && Equals(left, right._spool);
        }

        public static bool operator !=(CelloSpool left, UIViewSpool right)
        {
            return right != null && !Equals(left, right._spool);
        }

        public static bool operator !=(UIViewSpool left, UIViewSpool right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}