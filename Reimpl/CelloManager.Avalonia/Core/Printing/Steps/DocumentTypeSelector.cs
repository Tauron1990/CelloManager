using System.Threading.Tasks;
using CelloManager.Core.Printing.Workflow;
using CelloManager.Views.Orders;

namespace CelloManager.Core.Printing.Steps;

public sealed class DocumentTypeSelector : PrinterStep
{
    public override async ValueTask<StepId> OnExecute(PrinterContext context)
    {
        var type = await context.Dispatcher
            .InvokeAsync(
                () =>
                {
                    var window = new SelectDocumentTypeWindow();
                    return window.ShowDialog<DocumentType?>(App.MainWindow);
                });

        context.DocumentType = type;
        
        return type is null ? StepId.Finish : StepId.None;

    }
}