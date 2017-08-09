using System.Threading;
using Tauron.Application.CelloManager.Logic.Manager;
using Tauron.Application.Ioc;
using Tauron.Application.Models;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow
{
    [ExportViewModel(AppConststands.MainWindowName)]
    public sealed class MainWindowViewModel : ViewModelBase
    {
        private bool _refillInProgress;

        [Inject]
        public ISpoolManager SpoolManager { get; set; }

        [CommandTarget]
        public void Options()
        {
            var window = ViewManager.CreateWindow(AppConststands.OptionsWindow);
            window.ShowDialogAsync(CommonApplication.Current.MainWindow);
        }

        [CommandTarget]
        public bool CanRefill()
        {
            if (_refillInProgress) return true;
            return SpoolManager.IsRefillNeeded();
        }

        [CommandTarget]
        public void Refill()
        {
            _refillInProgress = true;
            SpoolManager.PrintOrder();
            _refillInProgress = false;

            InvalidateRequerySuggested();
        }
    }
}