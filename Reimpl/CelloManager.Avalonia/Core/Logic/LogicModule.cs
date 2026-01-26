using Microsoft.Extensions.DependencyInjection;

namespace CelloManager.Core.Logic;

public static class LogicModule
{
    public static void Config(IServiceCollection services)
    {
        services.AddScoped<SpoolManager>()
                .AddScoped<OrderManager>()
                .AddScoped<SpoolPriceManager>();
    }
}