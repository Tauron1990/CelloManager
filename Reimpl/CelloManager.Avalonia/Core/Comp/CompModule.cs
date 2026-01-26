using Microsoft.Extensions.DependencyInjection;

namespace CelloManager.Core.Comp;

public static class CompModule
{
    public static void Config(IServiceCollection services)
    {
        services.AddTransient<CoreDatabase>();
    }
}