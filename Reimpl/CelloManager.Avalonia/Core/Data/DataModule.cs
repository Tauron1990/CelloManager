using Microsoft.Extensions.DependencyInjection;

namespace CelloManager.Core.Data;

public static class DataModule
{
    public static void Config(IServiceCollection services)
    {
        services.AddSingleton<SpoolRepository>();
        services.AddSingleton<ErrorDispatcher>();
    }
}