using CelloManager.Core.Movere;
using CelloManager.Core.Comp;
using CelloManager.Core.Data;
using CelloManager.Core.Logic;
using CelloManager.Core.Printing;
using CelloManager.ViewModels;
using Jab;

namespace CelloManager;

[ServiceProvider]
[Import<IDataModule>]
[Import<IViewModelModule>]
[Import<ILogicModule>]
[Import<ICompModule>]
[Import<IPrintModule>]
internal partial class AppServiceProvider
{
    
}