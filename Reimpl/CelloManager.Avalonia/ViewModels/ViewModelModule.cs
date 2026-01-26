using CelloManager.ViewModels.Editing;
using CelloManager.ViewModels.Importing;
using CelloManager.ViewModels.Orders;
using CelloManager.ViewModels.SpoolDisplay;
using Microsoft.Extensions.DependencyInjection;

namespace CelloManager.ViewModels;

public static class ViewModelModule
{
    public static void Config(IServiceCollection collection)
    {
        collection.AddSingleton<MainWindowViewModel>()
                  .AddScoped<SpoolDisplayViewModel>()
                  .AddScoped<EditTabViewModel>()
                  .AddScoped<OrderDisplayViewModel>()
                  .AddScoped<ImportViewModel>();
    }
}