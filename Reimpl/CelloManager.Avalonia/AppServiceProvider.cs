using CelloManager.Core.Comp;
using CelloManager.Core.Data;
using CelloManager.Core.Logic;
using CelloManager.Core.Printing;
using CelloManager.Core.Printing.Internal;
using CelloManager.ViewModels;
using Jab;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TempFileStream;

namespace CelloManager;

[ServiceProvider]
[Import<IDataModule>]
[Import<IViewModelModule>]
[Import<ILogicModule>]
[Import<ICompModule>]
[Import<IPrintModule>]
[Import<IHelperModule>]

[Transient(typeof(ILogger<>), Factory = nameof(CreateLogger))]
[Singleton<IOptions<TempFileStreamConfig>>(Factory = nameof(CreateOpens))]
[Singleton<ILoggerFactory, LoggerFactory>]

internal partial class AppServiceProvider
{
    private ILogger<TCategory> CreateLogger<TCategory>(ILoggerFactory factory)
        => factory.CreateLogger<TCategory>();

    private IOptions<TempFileStreamConfig> CreateOpens()
        => new OptionsWrapper<TempFileStreamConfig>(new TempFileStreamConfig());
}