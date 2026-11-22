using System;
using System.Threading.Tasks;
using CelloManager.Core.Printing.Impl;
using CelloManager.Core.Printing.Workflow;
using QuestPDF.Infrastructure;

namespace CelloManager.Core.Printing.Steps;

public class DoPrint : PrinterStep
{
    public override async ValueTask<StepId> OnExecute(PrinterContext context)
    {
        if (context.DocumentType is null)
        {
            ErrorMessage = "No Document Type Selected";
            return StepId.Fail;
        }

        using IInternalDocument document = GenerateDocument(
            context.DocumentType.Value,
            GetOrThrow(context, static c => c.Document), 
            context);

        await document.Execute(context.Dispatcher, context.End ?? (static () => { })).ConfigureAwait(false);

        return StepId.None;
    }

    private static IInternalDocument GenerateDocument(DocumentType documentType, IDocument pages, PrinterContext context)
    {
        return (IInternalDocument)
        (
            documentType switch
            {
                DocumentType.Image => ImageDocument.GenerateDocument(pages, context),
                DocumentType.Pdf => PdfDocument.GenerateDocument(pages, context),
                DocumentType.Print => PrinterDocument.GenerateDocument(pages),
                DocumentType.Text => TextDocument.GenerateDocument(context),
                DocumentType.Mail => MailDocument.GenerateDocument(context),
                // ReSharper disable once LocalizableElement
                _ => throw new ArgumentOutOfRangeException(nameof(documentType), documentType, "The Document Type ist not Supported"),
            }
        );
    }
}