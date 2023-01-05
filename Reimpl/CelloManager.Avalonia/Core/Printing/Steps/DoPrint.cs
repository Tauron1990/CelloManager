using System;
using System.Linq;
using System.Threading.Tasks;
using CelloManager.Core.Printing.Impl;
using CelloManager.Core.Printing.Workflow;
using TempFileStream.Abstractions;

namespace CelloManager.Core.Printing.Steps;

public class DoPrint : PrinterStep
{
    public override async ValueTask<StepId> OnExecute(PrinterContext context)
    {
        if(context.DocumentType is null)
        {
            ErrorMessage = "No Document Type Selected";
            return StepId.Fail;
        }

        using IInternalDocument document = GenerateDocument(
            context.DocumentType.Value,
            context.TempFiles.GetFiles()
                .OrderBy(p => p.Key)
                .Select(p => p.Value)
                .ToArray());

        await document.Execute(context.Dispatcher, context.End ?? (static () => { }));
        
        return StepId.None;
    }
    
    private static IInternalDocument GenerateDocument(DocumentType documentType, ITempFile[] pages)
    {
        return (IInternalDocument)
        (
            documentType switch
            {
                DocumentType.Image => ImageDocument.GenerateDocument(pages),
                DocumentType.Pdf => PdfDocument.GenerateDocument(pages),
                DocumentType.Print => PrinterDocument.GenerateDocument(pages),
                // ReSharper disable once LocalizableElement
                _ => throw new ArgumentOutOfRangeException(nameof(documentType), documentType, "The Document Type ist not Supported"),
            }
        );
    }
}