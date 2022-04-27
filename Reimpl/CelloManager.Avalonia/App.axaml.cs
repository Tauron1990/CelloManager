using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CelloManager.Avalonia.Core.Data;
using CelloManager.Avalonia.ViewModels;
using CelloManager.Avalonia.Views;
using Splat;

namespace CelloManager.Avalonia
{
    public partial class App : Application
    {
        internal static readonly AppServiceProvider ServiceProvider = new();
        
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
                    DataContext = ServiceProvider.GetService<MainWindowViewModel>(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}