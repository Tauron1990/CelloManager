using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Xps.Packaging;
using CelloManager.Core.Printing;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace CelloManager.Windows;

public sealed class WindowsPrintingProvider : IPrintProvider
{
    private Application? _application;

    public async ValueTask RunPinting(IDocument document)
    {
        var xps = document.GenerateXps();
        var tempfile = Path.GetTempFileName();
        await File.WriteAllBytesAsync(tempfile, xps).ConfigureAwait(false);
        
        using var xpsDocument = new XpsDocument(tempfile, FileAccess.Read);

        var dispatcher = await GetDispatcher().ConfigureAwait(false);

        await dispatcher
           .InvokeAsync(() =>
                        {
                            // Create the print dialog object and set options
                            var pDialog = new PrintDialog
                                          {
                                              PageRangeSelection = PageRangeSelection.AllPages,
                                              UserPageRangeEnabled = true,
                                          };

                            // Display the dialog. This returns true if the user presses the Print button.
                            var print = pDialog.ShowDialog();
                            if (print != true) return;
                            var fixedDocSeq = xpsDocument.GetFixedDocumentSequence();
                            pDialog.PrintDocument(fixedDocSeq.DocumentPaginator, "CelloManger Bestellung Drucken");
                        });
    }

    public async ValueTask Shutdown()
    {
        if(_application is null)
            return;

        await _application.Dispatcher.InvokeAsync(_application.Shutdown);
    }

    private async ValueTask<Dispatcher> GetDispatcher()
    {
        if (_application is not null) return _application.Dispatcher;
        
        var dispatcherThread
            = new Thread(() =>
                         {
                             var app = new Application
                                       {
                                           ShutdownMode = ShutdownMode.OnExplicitShutdown,
                                       };
                             _application = app;
                             app.Run();
                         })
              {
                  IsBackground = false,
              };
        dispatcherThread.SetApartmentState(ApartmentState.STA);
        dispatcherThread.Start();
            
        while (true)
        {
            await Task.Delay(100).ConfigureAwait(false);
            if(_application is null) continue;
            await Task.Delay(500).ConfigureAwait(false);

            return _application.Dispatcher;
        }
    }
}