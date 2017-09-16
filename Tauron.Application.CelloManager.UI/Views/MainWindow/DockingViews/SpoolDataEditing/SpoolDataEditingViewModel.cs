using System;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using Syncfusion.UI.Xaml.Grid;
using Tauron.Application.CelloManager.Data.Manager;
using Tauron.Application.CelloManager.Logic.Manager;
using Tauron.Application.CelloManager.Resources;
using Tauron.Application.CelloManager.UI.Helper;
using Tauron.Application.Ioc;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews
{
    public sealed class SpoolDataEditingViewModel : DockingTabworkspace, ISpoolChangeNotifer
    {
        private IDisposable _operation;
        private bool _isEdited;

        [NotNull]
        [Inject]
        public ICelloRepository CelloRepository;

        [Inject]
        public IEventAggregator EventAggregator;

        public SpoolDataEditingViewModel() 
            : base(UIResources.LabelOptionsLayout, "SpoolDataEditingView")
        {
            
        }

        public UISyncObservableCollection<GuiEditSpool> Spools { get; } = new UISyncObservableCollection<GuiEditSpool>();

        [EventTarget]
        public void AddElement(AddNewRowInitiatingEventArgs args)
        {
            StartOperation();
            CelloSpoolBase spool = CelloRepository.Add();
            
            var ispool = new GuiEditSpool(this, spool);
            args.NewObject = ispool;

            InvalidateRequerySuggested();
        }

        [EventTarget]
        public void Remove(RecordDeletedEventArgs args)
        {
            StartOperation();
            foreach (var spool in args.Items.Cast<GuiEditSpool>())
                CelloRepository.Remove(spool.CelloSpoolBase);
        }

        [CommandTarget]
        public void Save()
        {
            _operation.Dispose();
            _operation = null;
            
            EventAggregator.GetEvent<ResetDatabaseEvent, EventArgs>().Publish(EventArgs.Empty);
        }

        [CommandTarget]
        public bool CanSave()
        {
            return _operation != null && _isEdited;
        }

        //public void 

        public void SpoolValueChanged(CelloSpoolBase spool)
        {
            _isEdited = true;
        }

        private void StartOperation()
        {
            if (_operation != null) return;

            _operation = CelloRepository.Manager.StartOperation();
        }

        public override void BuildCompled()
        {
            foreach (var spool in CelloRepository.GetSpools())
                Spools.Add(new GuiEditSpool(this, spool));
        }

        private void Refill()
        {
            using (Spools.BlockChangedMessages())
            {
                Spools.Clear();
                foreach (var spool in CelloRepository.GetSpools())
                    Spools.Add(new GuiEditSpool(this, spool));
            }
        }
    }
}