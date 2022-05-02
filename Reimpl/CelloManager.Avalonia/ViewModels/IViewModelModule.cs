using CelloManager.Avalonia.ViewModels.Editing;
using CelloManager.Avalonia.ViewModels.SpoolDisplay;
using Jab;

namespace CelloManager.Avalonia.ViewModels;

[ServiceProviderModule]
[Singleton(typeof(MainWindowViewModel))]
[Scoped(typeof(SpoolDisplayViewModel))]
[Scoped(typeof(EditTabViewModel))]
public interface IViewModelModule
{
    
}