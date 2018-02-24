using Syncfusion.Windows.Tools.Controls;
using Tauron.Application.CelloManager.Logic.Historie;
using Tauron.Application.CelloManager.Resources;
using Tauron.Application.CelloManager.UI.Helper;
using Tauron.Application.CelloManager.UI.Models;
using Tauron.Application.Models;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews.OrderView
{
    [ExportViewModel(AppConststands.OrderView)]
    public class OrderViewModel : DockingTabworkspace
    {
        [InjectModel(UIModule.OperationContextModelName)]
        public OperationContextModel OperationContext { private get; set; }

        [InjectModel(UIModule.SpoolModelName)]
        public SpoolModel SpoolModel { get; set; }

        [EventTarget(Synchronize = true)]
        public void ListClick()
        {
            if (SelectedRefill == null) return;

            var window = ViewManager.CreateWindow(AppConststands.OrderCompledWindow, SelectedRefill);

            window.ShowDialogAsync(MainWindow).ContinueWith(t =>
                                                            {
                                                                if (window.Result != null && (bool) window.Result)
                                                                    SpoolModel.OrderCompled(SelectedRefill);
                                                            });
        }

        public CommittedRefill SelectedRefill { get; set; }

        public OrderViewModel() : base(UIResources.OrderViewTitle)
        {
            SideInDockedMode = DockSide.Bottom;
            State = DockState.AutoHidden;
        }

        private bool _refillInProgress;

        
        [CommandTarget]
        public bool CanRefill()
        {
            if (OperationContext.IsOperationRunning) return false;
            return _refillInProgress || SpoolModel.IsRefillNeeded();
        }

        [CommandTarget]
        public void Refill()
        {
            _refillInProgress = true;
            SpoolModel.PlaceOrder();
            _refillInProgress = false;

            InvalidateRequerySuggested();
        }
    }
}