using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using JetBrains.Annotations;
using Tauron.Application.CelloManager.Data.Manager;
using Tauron.Application.CelloManager.Logic.Manager;
using Tauron.Application.Ioc;
using Tauron.Application.Models;

namespace Tauron.Application.CelloManager.UI.Views.OptionsWindow.SubWindows
{
    [ExportViewModel(AppConststands.LayoutOptionsView)]
    public sealed class SpoolOptionsViewModel : ViewModelBase, IOptionsPanel
    {
        //public abstract class NameLabel : ObservableObject
        //{
        //    private string _text;
        //    private readonly Action _changedHandler;

        //    protected NameLabel(string text, Action changedHandler)
        //    {
        //        _text = text;
        //        _changedHandler = changedHandler;
        //    }

        //    public string Text
        //    {
        //        get { return _text; }
        //        set
        //        {
        //            _text = value;
        //            Set(value);
        //            OnPropertyChanged();
        //            _changedHandler();
        //        }
        //    }

        //    protected void InternalSet(string text)
        //    {
        //        _text = text;
        //        OnPropertyChangedExplicit(nameof(Text));
        //    }

        //    protected abstract void Set(string text);
        //}

        //public sealed class TextNameLabel : NameLabel
        //{
        //    private readonly Action<string> _setter;

        //    public TextNameLabel(string text, Action changedHandler, Action<string> setter) : base(text, changedHandler)
        //    {
        //        _setter = setter;
        //    }

        //    protected override void Set(string text)
        //    {
        //        _setter(text);
        //    }
        //}

        //public sealed class IntNameLabel : NameLabel
        //{
        //    private readonly Action<int> _setter;

        //    public IntNameLabel(string text, Action changedHandler, Action<int> setter) : base(text, changedHandler)
        //    {
        //        _setter = setter;
        //    }

        //    protected override void Set(string text)
        //    {
        //        if (string.IsNullOrEmpty(text))
        //        {
        //            _setter(0);
        //            return;
        //        }

        //        try
        //        {
        //            _setter(int.Parse(text));
        //        }
        //        catch(Exception e) when(e is ArgumentException || e is FormatException || e is OverflowException)
        //        {
        //            _setter(0);
        //            InternalSet("0");
        //        }
        //    }
        //}

        public class InternalSpool : ObservableObject, INotifyDataErrorInfo
        {
            private readonly SpoolOptionsViewModel _model;

            public InternalSpool(SpoolOptionsViewModel model, CelloSpoolBase spool)
            {
                _model = model;
                CelloSpoolBase = spool;
                CelloSpoolBase.PropertyChanged += (sender, args) =>
                {
                    switch (args.PropertyName)
                    {
                        case nameof(Name):
                        case nameof(Type):
                        case nameof(Amount):
                        case nameof(Neededamount):
                            SpoolChanged();
                            break;
                    }
                };

                //Name = new TextNameLabel(spool.Name, SpoolChanged, s => spool.Name = s);
                //Type = new TextNameLabel(spool.Type, SpoolChanged, s => spool.Type = s);
                //Amount = new IntNameLabel(spool.Amount.ToString(), SpoolChanged, i => spool.Amount = i);
                //Neededamount = new IntNameLabel(spool.Neededamount.ToString(), SpoolChanged, i => spool.Neededamount = i);
            }

            public string Name
            {
                get => CelloSpoolBase.Name;
                set => CelloSpoolBase.Name = value;
            }

            public string Type
            {
                get => CelloSpoolBase.Type;
                set => CelloSpoolBase.Type = value;
            }

            public int Amount
            {
                get => CelloSpoolBase.Amount;
                set => CelloSpoolBase.Amount = value;
            }

            public int Neededamount
            {
                get => CelloSpoolBase.Neededamount;
                set => CelloSpoolBase.Neededamount = value;
            }

            public CelloSpoolBase CelloSpoolBase { get; }

            public IEnumerable GetErrors(string propertyName)
            {
                return CelloSpoolBase.GetErrors(propertyName);
            }

            public bool HasErrors => CelloSpoolBase.HasErrors;

            public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
            {
                add => CelloSpoolBase.ErrorsChanged += value;
                remove => CelloSpoolBase.ErrorsChanged -= value;
            }

            private void SpoolChanged()
            {
                _model.SpoolValueChanged(CelloSpoolBase);
            }
        }

        private bool _isEdited;
        private InternalSpool _selectedSpool;
        private IDisposable _operation;

        [NotNull]
        [Inject]
        public ICelloRepository CelloRepository;

        [Inject]
        public IEventAggregator EventAggregator;

        public SpoolOptionsViewModel()
        {
            Spools = new UISyncObservableCollection<InternalSpool>();
        }

        [UsedImplicitly]
        public UISyncObservableCollection<InternalSpool> Spools { get; }

        public InternalSpool SelectedSpool
        {
            get => _selectedSpool;
            set
            {
                _selectedSpool = value;
                OnPropertyChanged();
            }
        }

        protected override bool HasErrorOverride
        {
            get { return Spools.Any(s => s.HasErrors); }
        }

        public void Commit()
        {
            if (!IsEditing) return;

           EndEdit();
            CelloRepository.Manager.SaveChanges = true;
           _operation.Dispose();
            if (_isEdited)
                EventAggregator.GetEvent<ResetDatabaseEvent, EventArgs>().Publish(EventArgs.Empty);
        }

        public void Rollback()
        {
            if (!IsEditing) return;

            CancelEdit();

            CelloRepository.Manager.SaveChanges = false;
            _operation.Dispose();
        }

        public void Reset()
        {
            Rollback();
            using (Spools.BlockChangedMessages())
            {
                Spools.Clear();

                OpenDatabase();

                foreach (var entry in CelloRepository.GetSpools())
                    Spools.Add(new InternalSpool(this, entry));
            }
        }

        public void SpoolValueChanged(CelloSpoolBase spool)
        {
            OpenDatabase();

            //CelloRepository.Update(spool);
            _isEdited = true;
        }

        public void OpenDatabase()
        {
            if (IsEditing) return; //throw new InvalidOperationException("Multibe Database Openings: Layout Options");

            BeginEdit();
            _operation = CelloRepository.Manager.StartOperation();
        }

        [CommandTarget]
        public void RemoveElement()
        {
            var spool = SelectedSpool;
            if (spool == null) return;

            Spools.Remove(spool);
            OpenDatabase();

            CelloRepository.Remove(spool.CelloSpoolBase);
            _isEdited = true;
        }

        [CommandTarget]
        public bool CanRemoveElement()
        {
            return SelectedSpool != null;
        }

        [CommandTarget]
        public void AddElement()
        {
            CelloSpoolBase spool;
            var ok = CelloRepository.Add(string.Empty, string.Empty, 0, 0, out spool);

            if (!ok) return;

            var ispool = new InternalSpool(this, spool);

            Spools.Add(ispool);
            SelectedSpool = ispool;
            InvalidateRequerySuggested();
            _isEdited = true;
        }
    }
}