using System;
using System.Linq;
using JetBrains.Annotations;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.Windows.Tools.Controls;
using Tauron.Application.CelloManager.Data.Manager;
using Tauron.Application.CelloManager.Logic.Manager;
using Tauron.Application.CelloManager.Resources;
using Tauron.Application.CelloManager.UI.Helper;
using Tauron.Application.Ioc;
using Tauron.Application.Models;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews
{
    [ExportViewModel(AppConststands.SpoolDataEditingView)]
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
            CanClose = false;
            DesiredWidth = 750;
            CanAutoHide = true;
            State = DockState.AutoHidden;
            CanDocument = true;
        }

        public UISyncObservableCollection<GuiEditSpool> Spools { get; } = new UISyncObservableCollection<GuiEditSpool>();

        [EventTarget]
        public void AddElement(AddNewRowInitiatingEventArgs args)
        {
            StartOperation();
            CelloSpoolBase spool = CelloRepository.Add();

            var guiSpool = (GuiEditSpool) args.NewObject;
            guiSpool.Initialize(this, spool);
            //Spools.Add(guiSpool);

            _isEdited = true;

            InvalidateRequerySuggested();
        }

        [EventTarget]
        public void Remove(RecordDeletedEventArgs args)
        {
            StartOperation();
            foreach (var spool in args.Items.Cast<GuiEditSpool>())
                CelloRepository.Remove(spool.CelloSpoolBase);

            _isEdited = true;
            InvalidateRequerySuggested();
        }

        [CommandTarget]
        public void Save()
        {
            CelloRepository.Manager.SaveChanges = true;
            _operation.Dispose();
            _operation = null;
            
            EventAggregator.GetEvent<ResetDatabaseEvent, EventArgs>().Publish(EventArgs.Empty);
        }

        [CommandTarget]
        public bool CanSave()
        {
            return _operation != null && _isEdited;
        }

        [CommandTarget]
        public void Cancel()
        {
            if(_operation == null) return;

            CelloRepository.Manager.SaveChanges = false;
            _operation.Dispose();
            _operation = null;

            Refill();
        }

        [CommandTarget]
        public bool CanCancel()
        {
            return _isEdited;
        }

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