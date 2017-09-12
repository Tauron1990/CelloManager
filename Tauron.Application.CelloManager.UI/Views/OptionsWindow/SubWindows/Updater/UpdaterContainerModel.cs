using System;
using Tauron.Application.Models;

namespace Tauron.Application.CelloManager.UI.Views.OptionsWindow.SubWindows.Updater
{
    [ExportViewModel(AppConststands.UpdateOptionsView)]
    public class UpdaterContainerModel : ViewModelBase, IOptionsPanel
    {
        public AutoUpdaterViewModel AutoUpdaterViewModel { get; private set; } //= new AutoUpdaterViewModel();

        public event EventHandler LockUIEvent
        {
            add => AutoUpdaterViewModel.LockUIEvent += value;
            remove => AutoUpdaterViewModel.LockUIEvent -= value;
        }

        public void Reset()
        {
            AutoUpdaterViewModel.Reset();
        }

        public void Commit()
        {
            AutoUpdaterViewModel.Commit();
        }

        public void Rollback()
        {
            AutoUpdaterViewModel.Rollback();
        }

        public override void BuildCompled()
        {
            AutoUpdaterViewModel = Factory.Object<AutoUpdaterViewModel>();
            AutoUpdaterViewModel.ShutdownEvent += (sender, args) => CommonApplication.Current.Shutdown();
        }
    }
}