using System.IO;
using System.IO.Packaging;
using System.Windows.Controls;
using System.Windows.Xps.Packaging;
using Avalonia.Threading;
using CelloManager.Core.Printing;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace CelloManager.Windows;

public sealed class WindowsPrintingProvider : IPrintProvider
{
    public async ValueTask RunPinting(IDocument document, Dispatcher dispatcher)
    {
        var xps = document.GenerateXps();
        var pack = Package.Open(new MemoryStream(xps));
        using var xpsDocument = new XpsDocument(pack);

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
}