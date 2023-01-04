using CelloManager.Core.Printing.Impl;
using CelloManager.Core.Printing.Steps;
using Jab;

namespace CelloManager.Core.Printing;

[ServiceProviderModule]
[Transient<PrintBuilder>]
[Singleton<PrinterWorkflow>]
[Scoped<PrinterContext>]
public interface IPrintModule
{
    
}