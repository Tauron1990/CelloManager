using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.ReactiveUI;
using CelloManager.Core.Data;
using CelloManager.Core.Printing;
using CelloManager.Data;
using QuestPDF.Infrastructure;

namespace CelloManager
{
    public static class Bootstrapper
    {
        public static IPrintProvider? PrintProvider { get; set; }
        
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static async ValueTask StartApp(string[] args)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            
            await App.ServiceProvider.GetService<SpoolRepository>().Init().ConfigureAwait(true);

            try
            {
                BuildAvaloniaApp()
                    .StartWithClassicDesktopLifetime(args);
            }
            finally
            {
                await DataOperationManager.Manager.Shutdown().ConfigureAwait(false);
                if (PrintProvider is not null)
                    await PrintProvider.Shutdown().ConfigureAwait(false);
                await App.ServiceProvider.DisposeAsync().ConfigureAwait(false);
            }
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
    }
}