using System;
using System.ComponentModel;
using System.Text;
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

            SetName();
            StepCount = 1;
            
            _spool.PropertyChanged += SpoolOnPropertyChanged;

            AddCommand = new SimpleCommand(Add);
            RemoveCommand = new SimpleCommand(Remove);
        }

        public UIViewSpool()
            : this(new CelloSpool("Test", "64", 100, 200, -1), new UIDummySpoolManager(), null)
        {
        }

        public string FirstTwo { get; private set; }
        public string LastText { get; private set; }
    
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
            switch (propertyChangedEventArgs.PropertyName)
            {
                case nameof(_spool.Name):
                    SetName();

                    OnPropertyChangedExplicit(nameof(FirstTwo));
                    OnPropertyChangedExplicit(nameof(LastText));
                    break;
                case nameof(_spool.Amount):
                case nameof(_spool.Neededamount):
                    OnPropertyChangedExplicit(nameof(AmountText));
                    break;
            }
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

        private void SetName()
        {
            var first = new StringBuilder();
            var rest = new StringBuilder();

            bool setFirst = true;

            foreach (var nc in _spool.Name)
            {
                if (setFirst && char.IsDigit(nc))
                    first.Append(nc);
                else
                {
                    setFirst = false;
                    rest.Append(nc);
                }
            }

            FirstTwo = first.ToString();
            LastText = rest.ToString();
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