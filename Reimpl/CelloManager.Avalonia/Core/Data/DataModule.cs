using System.Diagnostics.CodeAnalysis;
using CelloManager.Data;
using Microsoft.Extensions.DependencyInjection;

namespace CelloManager.Core.Data;

public static class DataModule
{
    [RequiresUnreferencedCode("Data Base Access")]
    public static void Config(IServiceCollection services)
    {
        services.AddSingleton<DataOperationManager>();
        services.AddSingleton<SpoolRepository>();
        services.AddSingleton<ErrorDispatcher>();
    }
}