using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace CelloManager.Core.Comp;

[RequiresUnreferencedCode("Database (EF Core)")]
public static class CompModule
{
    public static void Config(IServiceCollection services)
    {
        services.AddTransient<CoreDatabase>();
    }
}