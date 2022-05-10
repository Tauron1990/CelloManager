using CelloManager.Avalonia.ViewModels.Editing;
using CelloManager.Avalonia.ViewModels.Importing;
using CelloManager.Avalonia.ViewModels.Orders;
using CelloManager.Avalonia.ViewModels.SpoolDisplay;
using Jab;

namespace CelloManager.Avalonia.ViewModels;

[ServiceProviderModule]
[Singleton(typeof(MainWindowViewModel))]
[Scoped(typeof(SpoolDisplayViewModel))]
[Scoped(typeof(EditTabViewModel))]
[Scoped(typeof(OrderDisplayViewModel))]
[Scoped(typeof(ImportViewModel))]
public interface IViewModelModule
{
    
}