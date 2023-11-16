using CelloManager.Core.Comp;
using CelloManager.Core.Data;
using CelloManager.Core.Logic;
using CelloManager.Core.Printing;
using CelloManager.Core.Printing.Internal;
using CelloManager.ViewModels;
using Jab;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Exceptions;
using Serilog.Extensions.Logging;
using Splat;
using Splat.Serilog;

namespace CelloManager;

[ServiceProvider]
[Import<IDataModule>]
[Import<IViewModelModule>]
[Import<ILogicModule>]
[Import<ICompModule>]
[Import<IPrintModule>]
[Import<IHelperModule>]

[Transient(typeof(ILogger<>), Factory = nameof(CreateLogger))]
//[Singleton<IOptions<TempFileStreamConfig>>(Factory = nameof(CreateOpens))]
[Singleton<ILoggerFactory>(Factory = nameof(ConfigurateLogger))]
[Singleton<AppManager>(Factory = nameof(GetManager))]
internal partial class AppServiceProvider
{
    private AppManager GetManager() => AppManager.Instance;
    
    private ILoggerFactory ConfigurateLogger()
    {
        var config = new LoggerConfiguration();

        config.Enrich.WithExceptionDetails();
#if DEBUG
        config.WriteTo.Async(c => c.Console());
#endif
        
        
        Log.Logger = config.CreateLogger();
        
        Locator.CurrentMutable.UseSerilogFullLogger();
        return new SerilogLoggerFactory(Log.Logger);
    }
    
    private ILogger<TCategory> CreateLogger<TCategory>(ILoggerFactory factory)
        => factory.CreateLogger<TCategory>();

//    private IOptions<TempFileStreamConfig> CreateOpens()
//        => new OptionsWrapper<TempFileStreamConfig>(new TempFileStreamConfig());
}