using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using CelloManager.ViewModels;
using CelloManager.Views;
using Microsoft.Extensions.DependencyInjection;

namespace CelloManager
{
    public sealed class App : Application
    {
        internal static IStorageProvider StorageProvider => MainWindow.StorageProvider;
        
        internal static readonly AppServiceProvider ServiceProvider = new();

        internal static Window MainWindow = null!;
        
        public override void Initialize() => AvaloniaXamlLoader.Load(this);

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    ViewModel = ServiceProvider.GetService<MainWindowViewModel>(),
                };
                
                MainWindow = desktop.MainWindow;
                MainWindow.Closed += (_, _) => desktop.Shutdown();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}