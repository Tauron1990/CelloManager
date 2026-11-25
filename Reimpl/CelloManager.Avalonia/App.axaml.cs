using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CelloManager.ViewModels;
using CelloManager.Views;
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable IL2026

namespace CelloManager;

public sealed class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(Bootstrapper.ServiceProvider, this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
                                 {
                                     ViewModel = Bootstrapper.ServiceProvider.GetRequiredService<MainWindowViewModel>(),
                                 };

            Window window = desktop.MainWindow;
            window.Closed += (_, _) => desktop.Shutdown();
            Bootstrapper.ServiceProvider.GetRequiredService<MainWindowProvider>().SetMainWindow(window);
        }

        base.OnFrameworkInitializationCompleted();
    }
}