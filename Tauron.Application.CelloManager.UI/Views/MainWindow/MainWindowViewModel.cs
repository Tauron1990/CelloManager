using Tauron.Application.CelloManager.Logic.Manager;
using Tauron.Application.CelloManager.UI.Models;
using Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews;
using Tauron.Application.Ioc;
using Tauron.Application.Models;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow
{
    [ExportViewModel(AppConststands.MainWindowName)]
    public sealed class MainWindowViewModel : TabWorkspaceHolderBase<TabWorkspace>
    {
        private bool _refillInProgress;

        [InjectModel(UIModule.OperationContextModelName)]
        public OperationContextModel OperationContext { get; set; }

        [Inject]
        public ISpoolManager SpoolManager { get; set; }

        //[CommandTarget]
        //public void Options()
        //{
        //    var window = ViewManager.CreateWindow(AppConststands.OptionsWindow);
        //    window.ShowDialogAsync(CommonApplication.Current.MainWindow);
        //}

        [CommandTarget]
        public bool CanRefill()
        {
            if (OperationContext.IsOperationRunning) return false;
            return _refillInProgress || SpoolManager.IsRefillNeeded();
        }

        [CommandTarget]
        public void Refill()
        {
            _refillInProgress = true;
            SpoolManager.PrintOrder();
            _refillInProgress = false;

            InvalidateRequerySuggested();
        }

        public override void BuildCompled()
        {
            Tabs.AddRange(new[]
            {
                Factory.Object<UpdaterContainerModel>()
            });
        }
    }
}