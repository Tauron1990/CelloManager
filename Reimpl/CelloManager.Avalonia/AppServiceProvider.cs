using CelloManager.Avalonia.Core.Comp;
using CelloManager.Avalonia.Core.Data;
using CelloManager.Avalonia.Core.Logic;
using CelloManager.Avalonia.ViewModels;
using CelloManager.Avalonia.Core.Movere;
using Jab;

namespace CelloManager.Avalonia;

[ServiceProvider]
[Import<IDataModule>]
[Import<IViewModelModule>]
[Import<ILogicModule>]
[Import<ICompModule>]
[Import<IPrintModule>]
internal partial class AppServiceProvider
{
    
}