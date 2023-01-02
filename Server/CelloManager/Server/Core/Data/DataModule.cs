using Microsoft.Extensions.DependencyInjection;

namespace CelloManager.Avalonia.Core.Data;

public static class DataModule
{
    public static void Add(IServiceCollection collection)
    {
        collection.AddSingleton<SpoolRepository>();
        collection.AddSingleton<ErrorDispatcher>();
    }
}