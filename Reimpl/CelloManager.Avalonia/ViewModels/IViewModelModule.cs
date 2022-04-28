using CelloManager.Avalonia.ViewModels.SpoolDisplay;
using Jab;

namespace CelloManager.Avalonia.ViewModels;

[ServiceProviderModule]
[Singleton(typeof(MainWindowViewModel))]
[Scoped(typeof(SpoolDisplayViewModel))]
public interface IViewModelModule
{
    
}