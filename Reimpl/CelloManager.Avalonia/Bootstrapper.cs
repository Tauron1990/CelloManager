using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.ReactiveUI;
using CelloManager.Core.Data;
using CelloManager.Data;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF;
using QuestPDF.Infrastructure;

namespace CelloManager;

public static class Bootstrapper
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread, RequiresUnreferencedCode("Access to Database (EF Core)")]
    public static async ValueTask StartApp(string[] args)
    {
        Settings.License = LicenseType.Community;

        DataOperationManager manager = App.ServiceProvider.GetRequiredService<DataOperationManager>();
        await App.ServiceProvider.GetRequiredService<SpoolRepository>().Init().ConfigureAwait(true);

        try
        {
            BuildAvaloniaApp()
               .StartWithClassicDesktopLifetime(args);
        }
        finally
        {
            await manager.Shutdown().ConfigureAwait(false);
            await App.ServiceProvider.DisposeAsync().ConfigureAwait(false);
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
                     .UsePlatformDetect()
                     .LogToTrace()
                     .UseReactiveUI();
}