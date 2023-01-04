using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using CelloManager.Core.Data;
using CelloManager.Core.Printing.Impl;
using CelloManager.Core.Printing.Steps;
using CelloManager.Core.Printing.Workflow;
using Microsoft.Extensions.DependencyInjection;

namespace CelloManager.Core.Printing;

public sealed class PrintBuilder
{
    /*private IPrintDocument GenerateDocument(DocumentType documentType, PendingOrderPrintView view)
    {
        return documentType switch
        {
            DocumentType.Image => ImageDocument.GenerateDocument(view),
            DocumentType.PDF => PdfDocument.GenerateDocument(view),
            DocumentType.Print => PrinterDocument.GenerateDocument(view),
            _ => throw new ArgumentOutOfRangeException(nameof(documentType), documentType, "The Document Type ist not Supported"),
        };
    }

    private async ValueTask ExecuteDocument(IPrintDocument document, Action end)
    {
        if(document is not IInternalDocument internalDocument)
            throw new InvalidOperationException("The Document is not Compatible");

        await internalDocument.Execute(Dispatcher.UIThread, end);
    }
    
    public async ValueTask StartPrint(PendingOrderViewModel model, PendingOrderPrintView pendingOrderPrintView)
    {
        var type = await Dispatcher.UIThread
            .InvokeAsync(
                () =>
                {
                    var window = new SelectDocumentTypeWindow();
                    return window.ShowDialog<DocumentType?>(App.MainWindow);
                });

        if(type is null)
        {
            model.Close();
            return;
        }
        
        using IPrintDocument doc = GenerateDocument(type.Value, pendingOrderPrintView);
        await ExecuteDocument(doc, model.Close);
    }*/

    public async ValueTask PrintPendingOrder(PendingOrder order, Dispatcher dispatcher, IServiceProvider serviceProvider)
    {
        var errors = serviceProvider.GetRequiredService<ErrorDispatcher>();
        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        
        try
        {
            var workflow = scope.ServiceProvider.GetRequiredService<PrinterWorkflow>();
            var printerContext = scope.ServiceProvider.GetRequiredService<PrinterContext>();

            printerContext.Dispatcher = dispatcher;
            printerContext.ServiceProvider = scope.ServiceProvider;
            printerContext.Order = order;
            
            await workflow.Begin(StepId.Start, printerContext);
        }
        catch (Exception e)
        {
            errors.Send(e);
        }
    }
}