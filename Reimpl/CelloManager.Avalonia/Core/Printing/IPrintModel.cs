using CelloManager.Core.Printing.Impl;
using CelloManager.Core.Printing.Steps;
using Jab;

namespace CelloManager.Core.Printing;

[ServiceProviderModule]

[Transient<PrintBuilder>]
[Transient<PrintProgressManager>]

[Singleton<PrinterWorkflow>]
[Scoped<PrinterContext>]
public interface IPrintModule
{
    
}