using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.ReactiveUI;
using CelloManager.Core.Data;
using CelloManager.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuestPDF;
using QuestPDF.Infrastructure;

namespace CelloManager;

[RequiresUnreferencedCode("Access to Database (EF Core)")]
public static class Bootstrapper
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static async ValueTask StartApp(string[] args)
    {
        ILogger logger = ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(Bootstrapper));
        DataOperationManager? manager = null;
        logger.LogInformation(new EventId(1, "ApplicationStartupStarted"), "Application startup initiated");

        try
        {
            Settings.License = LicenseType.Community;

            manager = ServiceProvider.GetRequiredService<DataOperationManager>();
            await ServiceProvider.GetRequiredService<SpoolRepository>().Init().ConfigureAwait(true);
            logger.LogInformation(new EventId(2, "ApplicationStartupCompleted"), "Application initialization completed");

            BuildAvaloniaApp()
               .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception exception)
        {
            logger.LogCritical(new EventId(3, "ApplicationStartupFailed"), exception, "Application startup or execution failed");
            throw;
        }
        finally
        {
            logger.LogInformation(new EventId(4, "ApplicationShutdownStarted"), "Application shutdown initiated");
            try
            {
                await ShutdownManager(manager, logger).ConfigureAwait(false);
            }
            finally
            {
                logger.LogInformation(new EventId(6, "ApplicationShutdownCompleted"), "Application shutdown cleanup completed");
                await ServiceProvider.DisposeAsync().ConfigureAwait(false);
            }
        }
    }

    private static async ValueTask ShutdownManager(DataOperationManager? manager, ILogger logger)
    {
        if (manager is null)
        {
            return;
        }

        try
        {
            await manager.Shutdown().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            logger.LogError(new EventId(5, "DatabaseWorkerShutdownFailed"), exception, "Database worker shutdown failed");
            throw;
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
                     .UsePlatformDetect()
                     .UseReactiveUI();

    internal static readonly AppServiceProvider ServiceProvider = new();
}