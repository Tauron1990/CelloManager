using CelloManager.Avalonia.Core.Data;
using CelloManager.Avalonia.Core.Logic;
using CelloManager.Avalonia.ViewModels;
using Jab;

namespace CelloManager.Avalonia;

[ServiceProvider]
[Import(typeof(IDataModule))]
[Import(typeof(IViewModelModule))]
[Import(typeof(ILogicModule))]
internal partial class AppServiceProvider
{
    
}