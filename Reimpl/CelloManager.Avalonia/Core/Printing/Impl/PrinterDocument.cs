using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Threading;
using CelloManager.Core.Movere.ViewModels;
using CelloManager.Core.Movere.Views;
using CelloManager.Views.Orders;

namespace CelloManager.Core.Printing.Impl;

public sealed class PrinterDocument : IInternalDocument
{
    private readonly PrintDocument _printDocument;
    private readonly string _fileName = Path.GetTempFileName();
    
    private Stream? _data;

    public DocumentType Type => DocumentType.Print;

    private PrinterDocument(PrintDocument printDocument) 
        => _printDocument = printDocument;

    public static IPrintDocument GenerateDocument(PendingOrderPrintView view)
    {
        var doc = new PrintDocument();
        
        var printerDoc = new PrinterDocument(doc);
        doc.PrintPage += DocumentOnPrintPage;
        
        return printerDoc;

        void DocumentOnPrintPage(object sender, PrintPageEventArgs e)
        {
            if(printerDoc._data is null)
            {
                var stream = new FileStream(printerDoc._fileName, FileMode.Create, FileAccess.ReadWrite);
                view.RenderTo(stream);
                printerDoc._data = stream;
            }

            printerDoc._data.Position = 0;
            using Image? bitmap = Image.FromStream(printerDoc._data);

            e.Graphics.DrawImage(bitmap, 10, 10);
        }
    }

    public async ValueTask Execute(Dispatcher dispatcher, Action end)
    {
        TaskCompletionSource<bool> result = new();
        var dialogTask = Task.CompletedTask;

        await Dispatcher.UIThread.InvokeAsync(
            () =>
            {
                var dialog = new PrintDialog();
                var model = new PrintDialogViewModel(
                    _printDocument,
                    b =>
                    {
                        dialog.Close();
                        result.SetResult(b);
                    });
                dialog.ViewModel = model;

                dialogTask = dialog.ShowDialog(App.MainWindow);
            });

        await dialogTask;

        if(await result.Task)
        {
            _printDocument.EndPrint += (_, _) => end();
            _printDocument.Print();
        }
        else
            end();
    }

    public void Dispose()
    {
        _printDocument.Dispose();
        _data?.Dispose();
        _data = null;
        
        if(File.Exists(_fileName))
            File.Delete(_fileName);
    }
}