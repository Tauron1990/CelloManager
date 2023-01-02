using CelloManager.Avalonia.ViewModels.Editing;
using CelloManager.Avalonia.ViewModels.Importing;
using CelloManager.Avalonia.ViewModels.Orders;
using CelloManager.Avalonia.ViewModels.SpoolDisplay;
using Jab;

namespace CelloManager.Avalonia.ViewModels;

[ServiceProviderModule]
[Singleton<MainWindowViewModel>]
[Scoped<SpoolDisplayViewModel>]
[Scoped<EditTabViewModel>]
[Scoped<OrderDisplayViewModel>]
[Scoped<ImportViewModel>]
public interface IViewModelModule
{
    
}