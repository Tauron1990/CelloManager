using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CelloManager.Core.Printing.Workflow;
using CelloManager.Views;
using CelloManager.Views.Orders;

namespace CelloManager.Core.Printing.Steps;

public sealed class DocumentTypeSelector : PrinterStep
{
    public override async ValueTask<StepId> OnExecute(PrinterContext context)
    {
        DocumentType? type = await context.Dispatcher
                                          .InvokeAsync(
                                               () =>
                                               {
                                                   MainWindowProvider provider = context.ServiceProvider.GetRequiredService<MainWindowProvider>();
                                                   SelectDocumentTypeWindow window = new();
                                                   return window.ShowDialog<DocumentType?>(provider.MainWindow);
                                               }).ConfigureAwait(false);

        context.DocumentType = type;

        if (type is null)
        {
            context.ServiceProvider.GetRequiredService<ILogger<DocumentTypeSelector>>()
                   .LogInformation(new EventId(4003, "DocumentSelectionCanceled"), "Document type selection was canceled");
        }

        return type is null ? StepId.Finish : StepId.None;
    }
}