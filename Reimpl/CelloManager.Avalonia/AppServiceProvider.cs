using CelloManager.Avalonia.Core.Data;
using CelloManager.Avalonia.ViewModels;
using Jab;

namespace CelloManager.Avalonia;

[ServiceProvider]
[Import(typeof(IDataModule))]
[Import(typeof(IViewModelModule))]
internal partial class AppServiceProvider
{
    
}