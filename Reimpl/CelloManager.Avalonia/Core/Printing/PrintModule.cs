using CelloManager.Core.Printing.Impl;
using CelloManager.Core.Printing.Steps;
using Microsoft.Extensions.DependencyInjection;

namespace CelloManager.Core.Printing;

public static class PrintModule
{
    public static void Config(IServiceCollection services)
    {
        services.AddSingleton<PrinterWorkflow>()
                .AddScoped<PrinterContext>();
    }
}