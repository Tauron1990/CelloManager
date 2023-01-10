using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.ReactiveUI;
using CelloManager.Core.Data;

namespace CelloManager
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static async Task Main(string[] args)
        {
            Akavache.Registrations.Start("CelloManager");
            await App.ServiceProvider.GetService<SpoolRepository>().Init();

            try
            {
                BuildAvaloniaApp()
                    .StartWithClassicDesktopLifetime(args);
            }
            finally
            {
                await App.ServiceProvider.DisposeAsync();
                await Akavache.BlobCache.Shutdown();
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