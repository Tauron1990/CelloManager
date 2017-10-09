using Syncfusion.Windows.Tools.Controls;
using Tauron.Application.CelloManager.Resources;
using Tauron.Application.CelloManager.UI.Helper;
using Tauron.Application.CelloManager.UI.Models;
using Tauron.Application.CelloManager.UI.Views.OptionsWindow.SubWindows;
using Tauron.Application.Models;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews
{
    [ExportViewModel(AppConststands.UpdateView)]
    public class UpdaterContainerModel : DockingTabworkspace
    {
        public UpdaterContainerModel()
            : base(UIResources.LabelOptionsAutoUpdate, "UpdaterView")
        {
            CanClose = false;
            DesiredWidth = 600;
            CanAutoHide = true;
        }

        [InjectModel(UIModule.OperationContextModelName)]
        public OperationContextModel OperationContext { get; set; }

        public AutoUpdaterViewModel AutoUpdaterViewModel { get; private set; } //= new AutoUpdaterViewModel();
        
        //public void Reset()
        //{
        //    AutoUpdaterViewModel.Reset();
        //}

        //public void Commit()
        //{
        //    AutoUpdaterViewModel.Commit();
        //}

        //public void Rollback()
        //{
        //    AutoUpdaterViewModel.Rollback();
        //}

        public override void BuildCompled()
        {
            AutoUpdaterViewModel = Factory.Object<AutoUpdaterViewModel>();
            AutoUpdaterViewModel.ShutdownEvent += (sender, args) => CommonApplication.Current.Shutdown();
            AutoUpdaterViewModel.LockUIEvent += (sender, args) => OperationContext.IsOperationRunning = true;
            InvalidateRequerySuggested();
        }
    }
}