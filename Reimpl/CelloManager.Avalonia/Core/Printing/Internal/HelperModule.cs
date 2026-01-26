using Microsoft.Extensions.DependencyInjection;

namespace CelloManager.Core.Printing.Internal;

public static class HelperModule
{
    public static void Config(IServiceCollection services)
    {
        services.AddScoped<PrintUiModel>();
    }
}