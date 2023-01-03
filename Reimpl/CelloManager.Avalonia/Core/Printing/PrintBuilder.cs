using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using CelloManager.Core.Printing.Impl;
using CelloManager.Views.Orders;

namespace CelloManager.Core.Printing;

public sealed class PrintBuilder
{
    public IPrintDocument GenerateDocument(DocumentType documentType, PendingOrderPrintView view)
    {
        return documentType switch
        {
            DocumentType.Image => ImageDocument.GenerateDocument(view),
            DocumentType.PDF => PdfDocument.GenerateDocument(view),
            DocumentType.Print => PrinterDocument.GenerateDocument(view),
            _ => throw new ArgumentOutOfRangeException(nameof(documentType), documentType, "The Document Type ist not Supported"),
        };
    }

    public async ValueTask ExecuteDocument(IPrintDocument document, Action end)
    {
        if(document is not IInternalDocument internalDocument)
            throw new InvalidOperationException("The Document is not Compatible");

        await internalDocument.Execute(Dispatcher.UIThread, end);
    }
}