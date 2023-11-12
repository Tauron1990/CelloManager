using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace CelloManager.Core.Printing.Impl;

public sealed class PrinterDocument : IInternalDocument
{
    private readonly IDocument _printDocument;


    public DocumentType Type => DocumentType.Print;

    private PrinterDocument(IDocument printDocument)
    {
        _printDocument = printDocument;
    }

    public void Dispose() {}

    public static IPrintDocument GenerateDocument(IDocument pages) 
        => new PrinterDocument(pages);

    public async ValueTask Execute(Dispatcher dispatcher, Action end)
    {
        try
        {
            if (Bootstrapper.PrintProvider is not null)
                await Bootstrapper.PrintProvider.RunPinting(_printDocument).ConfigureAwait(false);
            else
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
        /*var appData = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Tauron",
            "CelloManager");

        if (!Directory.Exists(appData))
            Directory.CreateDirectory(appData);

        var file = Path.Combine(appData, "Temp.pdf");

        var stream = File.OpenWrite(file);
        await using (stream.ConfigureAwait(false))
        {
            _printDocument.GeneratePdf(stream);
        }

        using var process = Process.Start(file);
        using var cancel = new CancellationTokenSource(TimeSpan.FromMinutes(2));

        await process.WaitForExitAsync(cancel.Token).ConfigureAwait(false);*/
    }
}