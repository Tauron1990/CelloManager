using System;
using System.ComponentModel;
using System.Windows.Input;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.CelloManager.Logic.Manager;
using Tauron.Application.CelloManager.Resources;
using Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews.Helper;
using Tauron.Application.Commands;
using Tauron.Application.Models;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews
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
            RegisterInheritedModel("CelloSpool", spool);
            EditingInheritedModel = true;

            _spool.PropertyChanged += SpoolOnPropertyChanged;
            StepCount = 1;

            AddCommand = new SimpleCommand(Add);
            RemoveCommand = new SimpleCommand(Remove);
        }

        public UIViewSpool()
            : this(new UIDummyCelloSpool(), new UIDummySpoolManager())
        {
        }

        public string FirstTwo => _spool.Name.Substring(0, 2);
        public string LastText => _spool.Name.Substring(2, _spool.Name.Length - 2);

        public string AmountText => _spool.Amount + " " + UIResources.CommonLabelOf + " " + _spool.Neededamount;

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

            _manager.UpdateSpools(new []{GetAmoutUpdaterAction()});
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

        public Action<IUnitOfWork> GetAmoutUpdaterAction()
        {
            return u =>
            {
                _spool.UpdateSpool(u);
                
                OnPropertyChangedExplicit(nameof(AmountText));
            };
        }

        private int GetStepCount()
        {
            var temp = StepCount;

            StepCount = 1;

            return temp;
        }

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