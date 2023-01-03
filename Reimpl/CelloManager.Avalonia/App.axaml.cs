using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CelloManager.ViewModels;
using CelloManager.Views;

namespace CelloManager
{
    public partial class App : Application
    {
        internal static readonly AppServiceProvider ServiceProvider = new();

        internal static Window MainWindow = null!;
        
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    ViewModel = ServiceProvider.GetService<MainWindowViewModel>(),
                };
                MainWindow = desktop.MainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}