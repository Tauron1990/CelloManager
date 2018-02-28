using Tauron.Application.CelloManager.Logic;
using Tauron.Application.Ioc;
using Tauron.Application.Models;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.Windows.Options
{
    [ExportViewModel(AppConststands.OptionsWindow)]
    public class OptionsWindowViewModel : ViewModelBase
    {
        public IWindow Window { get; set; }

        [Inject]
        public ISettingsModel Settings { get; set; }

        [CommandTarget]
        public void Save()
        {
            Settings.Save();
            Window.DialogResult = true;
        }

        [CommandTarget]
        public bool CanSave() => Settings.CanSave();

        [CommandTarget]
        public void Cancel()
        {
            Settings.Cancel();
            Window.DialogResult = false;
        }

        public override void BuildCompled()
        {
            Settings.ErrorsChanged += (sender, args) =>
                                      {
                                          OnPropertyChangedExplicit(nameof(ErrorText));
                                          InvalidateRequerySuggested();
                                      };
            base.BuildCompled();
        }

        public string ErrorText => Settings.ErrorText;
        
        public override void OnShow(IWindow window)
        {
            Window = window;
            base.OnShow(window);
        }
    }
}