using System;
using System.Threading.Tasks;
using CelloManager.Core.Comp;
using CelloManager.Core.Data;
using CelloManager.Core.Logic;
using CelloManager.Core.Printing;
using CelloManager.Core.Printing.Internal;
using CelloManager.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

namespace CelloManager;

internal sealed class AppServiceProvider : IServiceProvider, IDisposable, IAsyncDisposable
{
    private readonly ServiceProvider _serviceProvider;

    public AppServiceProvider()
    {
        ServiceCollection collection = new();

        DataModule.Config(collection);
        ViewModelModule.Config(collection);
        LogicModule.Config(collection);
        CompModule.Config(collection);
        PrintModule.Config(collection);
        HelperModule.Config(collection);

        collection.AddLogging(ConfigurateLogger);
        collection.AddSingleton(() => AppManager.Instance);

        _serviceProvider = collection.BuildServiceProvider();
    }

    public ValueTask DisposeAsync() => _serviceProvider.DisposeAsync();
    public void Dispose() => _serviceProvider.Dispose();


    public object? GetService(Type serviceType) => _serviceProvider.GetService(serviceType);

    private void ConfigurateLogger(ILoggingBuilder builder)
    {
        LoggerConfiguration config = new();

        config.Enrich.WithExceptionDetails();
        config.WriteTo.Async(c => c.File(
                                 "Log/log.txt",
                                 LogEventLevel.Information,
                                 rollingInterval: RollingInterval.Day,
                                 retainedFileCountLimit: 3));
        #if DEBUG
        config.WriteTo.Async(c => c.Console());
        #endif


        Log.Logger = config.CreateLogger();

        builder.ClearProviders();
        builder.AddSerilog(dispose: true);
    }
}