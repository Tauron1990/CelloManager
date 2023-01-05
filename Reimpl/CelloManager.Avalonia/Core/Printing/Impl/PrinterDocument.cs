using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Threading;
using CelloManager.Core.Movere.ViewModels;
using CelloManager.Core.Movere.Views;
using CelloManager.Views.Orders;
using TempFileStream.Abstractions;

namespace CelloManager.Core.Printing.Impl;

public sealed class PrinterDocument : IInternalDocument
{
    private readonly PrintDocument _printDocument;
    private int _currentIndex;
    
    public DocumentType Type => DocumentType.Print;

    private PrinterDocument(PrintDocument printDocument) 
        => _printDocument = printDocument;

    public static IPrintDocument GenerateDocument(ITempFile[] view)
    {
        var doc = new PrintDocument();
        
        var printerDoc = new PrinterDocument(doc);
        doc.PrintPage += DocumentOnPrintPage;
        
        return printerDoc;

        void DocumentOnPrintPage(object sender, PrintPageEventArgs e)
        {
            using Image? img = Image.FromStream(view[printerDoc._currentIndex].CreateReadStream());
            e.Graphics.DrawImage(img, 10, 10);

            printerDoc._currentIndex++;
            e.HasMorePages = printerDoc._currentIndex < view.Length;
            
            if(e.HasMorePages) return;

            printerDoc._currentIndex = 0;
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
        _currentIndex = -1;
    }
}