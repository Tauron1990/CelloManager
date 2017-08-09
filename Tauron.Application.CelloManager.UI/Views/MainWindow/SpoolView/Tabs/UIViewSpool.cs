using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Input;
using Tauron.Application.CelloManager.Logic.Manager;
using Tauron.Application.CelloManager.Resources;
using Tauron.Application.CelloManager.UI.Views.MainWindow.SpoolView.Tabs.Helper;
using Tauron.Application.Commands;
using Tauron.Application.Models;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.SpoolView.Tabs
{
    //[ExportViewModel(AppConststands.SingleUISpoolViwName)]
    public class UIViewSpool : ViewModelBase, IEquatable<UIViewSpool>
    {
        private readonly ISpoolManager _manager;
        private CelloSpoolBase _spool;
        private int _stepCount;

        public UIViewSpool(CelloSpoolBase spool, ISpoolManager manager)
        {
            _spool = spool;
            _manager = manager;

            _spool.PropertyChanged += SpoolOnPropertyChanged;
            StepCount = 1;

            AddCommand = new SimpleCommand(Add);
            RemoveCommand = new SimpleCommand(Remove);
        }

        public UIViewSpool()
            : this(new UIDummyCelloSpool(), new UIDummySpoolManager())
        {
        }

        public string FirstTwo => Name.Substring(0, 2);
        public string LastText => Name.Substring(2, Name.Length - 2);

        public string AmountText => Amount + " " + UIResources.CommonLabelOf + " " + Neededamount;

        public int StepCount
        {
            get => _stepCount;
            set
            {
                _stepCount = value;
                OnPropertyChanged();
            }
        }

        public string UniquieId => _spool.UniquieId;

        public override string ToString()
        {
            return $"Name: {Name} + Type: {Type}";
        }

        private void SpoolOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            OnPropertyChanged(propertyChangedEventArgs);
            if (propertyChangedEventArgs.PropertyName == nameof(Name))
            {
                OnPropertyChangedExplicit(nameof(FirstTwo));
                OnPropertyChangedExplicit(nameof(LastText));
            }

            if (propertyChangedEventArgs.PropertyName == nameof(Amount) || propertyChangedEventArgs.PropertyName == nameof(Neededamount))
                OnPropertyChangedExplicit(nameof(AmountText));
        }

        public ICommand AddCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }

        public void Add(object var)
        {
            _manager.AddSpool(_spool, GetStepCount());
        }
        
        public void Remove(object var)
        {
            var count = GetStepCount();

            for (var i = 0; i < count; i++)
                _manager.SpoolEmty(_spool);
        }

        public Action GetAmoutUpdaterAction()
        {
            return () =>
            {
                _spool.UpdateSpool();
                
                OnPropertyChangedExplicit(nameof(Amount));
                OnPropertyChangedExplicit(nameof(AmountText));
            };
        }

        private int GetStepCount()
        {
            var temp = StepCount;

            StepCount = 1;

            return temp;
        }

        #region ICelloImpl

        public void BeginEdit()
        {
            _spool.BeginEdit();
        }

        public void EndEdit()
        {
            _spool.EndEdit();
        }

        public void CancelEdit()
        {
            _spool.CancelEdit();
        }

        public IEnumerable GetErrors(string propertyName)
        {
            return _spool.GetErrors(propertyName);
        }

        public bool HasErrors => _spool.HasErrors;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
        {
            add => _spool.ErrorsChanged += value;
            remove => _spool.ErrorsChanged -= value;
        }

        public string Name
        {
            get => _spool.Name;
            set => throw new InvalidOperationException();
        }

        public string Type
        {
            get => _spool.Type;
            set => throw new InvalidOperationException();
        }

        public int Amount
        {
            get => _spool.Amount;
            set => throw new InvalidOperationException();
        }

        public int Neededamount
        {
            get => _spool.Neededamount;
            set => throw new InvalidOperationException();
        }

        #endregion

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

        public static bool operator ==(CelloSpoolBase left, UIViewSpool right)
        {
            return Equals(left, right._spool);
        }

        public static bool operator !=(CelloSpoolBase left, UIViewSpool right)
        {
            return !Equals(left, right._spool);
        }

        public static bool operator !=(UIViewSpool left, UIViewSpool right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}