using System.Linq;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.Windows.Tools.Controls;
using Tauron.Application.CelloManager.Logic.Manager;
using Tauron.Application.CelloManager.Resources;
using Tauron.Application.CelloManager.UI.Helper;
using Tauron.Application.CelloManager.UI.Models;
using Tauron.Application.Models;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews
{
    [ExportViewModel(AppConststands.SpoolDataEditingView)]
    public sealed class SpoolDataEditingViewModel : DockingTabworkspace
    {
        [InjectModel(UIModule.OperationContextModelName)]
        public OperationContextModel OperationContextModel { get; set; }

        [InjectModel(UIModule.SpoolModelName)]
        public SpoolModel SpoolModel { get; set; }

        public SpoolDataEditingViewModel() 
            : base(UIResources.LabelOptionsLayout, AppConststands.SpoolDataEditingView)
        {
            CanClose = true;
            DesiredWidth = 750;
            CanAutoHide = true;
            CanDocument = true;
            State = DockState.AutoHidden;
        }

        public UISyncObservableCollection<GuiEditSpool> Spools { get; } = new UISyncObservableCollection<GuiEditSpool>();

        [EventTarget]
        public void AddElement(AddNewRowInitiatingEventArgs args)
        {
            var uiElement = (GuiEditSpool) args.NewObject;
            var editSpool = new EditSpool(new CelloSpool());

            uiElement.Initialize(editSpool);
            SpoolModel.Add(editSpool);
        }

        [EventTarget]
        public void Remove(RecordDeletedEventArgs args)
        {
            foreach (var spool in args.Items.Cast<GuiEditSpool>())
                SpoolModel.Remove(spool.EditSpool);
        }

        [CommandTarget]
        public void Save() => Stop(true);

        [CommandTarget]
        public bool CanSave() => SpoolModel.IsInEditing;

        [CommandTarget]
        public void Cancel() => Stop(false);

        [CommandTarget]
        public bool CanCancel() => SpoolModel.IsInEditing;

        [CommandTarget]
        public void BeginEditData()
        {
            OperationContextModel.IsEditingOperationRunning = true;
            SpoolModel.EnterEditMode();

            foreach (var editSpool in SpoolModel.EditorSpools)
                Spools.Add(new GuiEditSpool(editSpool));
        }

        [CommandTarget]
        public bool CanBeginEditData() => !SpoolModel.IsInEditing;

        private void Stop(bool flag)
        {
            OperationContextModel.IsOperationRunning = true;
            SpoolModel.ExitEditMode(flag);
            Spools.Clear();
            OperationContextModel.IsEditingOperationRunning = false;
            OperationContextModel.IsOperationRunning = false;
        }
    }
}