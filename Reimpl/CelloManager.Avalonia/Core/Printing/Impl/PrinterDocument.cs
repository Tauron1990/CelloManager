using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace CelloManager.Core.Printing.Impl;

public sealed class PrinterDocument : IInternalDocument
{
    private readonly IDocument _printDocument;

    private PrinterDocument(IDocument printDocument) => _printDocument = printDocument;


    public DocumentType Type => DocumentType.Print;

    public void Dispose()
    {
    }

    public static IPrintDocument GenerateDocument(IDocument pages)
        => new PrinterDocument(pages);

    public async ValueTask Execute(Dispatcher dispatcher, Action end)
    {
        try
        {
            await NoProvider().ConfigureAwait(false);
        }
        finally
        {
            end();
        }
    }

    private ValueTask NoProvider()
    {
        _printDocument.GeneratePdfAndShow();
        return ValueTask.CompletedTask;
    }
}