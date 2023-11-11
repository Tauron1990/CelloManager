using CelloManager.ViewModels.Editing;
using CelloManager.ViewModels.Importing;
using CelloManager.ViewModels.Orders;
using CelloManager.ViewModels.SpoolDisplay;
using Jab;

namespace CelloManager.ViewModels;

[ServiceProviderModule]
[Singleton<MainWindowViewModel>]
[Scoped<SpoolDisplayViewModel>]
[Scoped<EditTabViewModel>]
[Scoped<OrderDisplayViewModel>]
[Scoped<ImportViewModel>]
//[Transient<PrintDialogViewModel>]
public interface IViewModelModule
{
    
}