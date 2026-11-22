using System;
using System.Threading.Tasks;
using Avalonia.Input.Platform;
using Avalonia.Threading;
using CelloManager.Core.Printing.Steps;
using ThingLing.Controls;

namespace CelloManager.Core.Printing.Impl;

public sealed class TextDocument(PrinterContext context) : IInternalDocument
{
    public static IInternalDocument GenerateDocument(PrinterContext context) 
        => new TextDocument(context);

    public void Dispose()
    {
        
    }

    public DocumentType Type => DocumentType.Text;

    public async ValueTask Execute(Dispatcher dispatcher, Action end)
    {
        try
        {
            string result = TextGenerator.RenderOrder(context);

            await dispatcher.InvokeAsync(
                async () =>
                {
                    IClipboard? clipboard = App.MainWindow.Clipboard;
                    string message;
                    
                    if (clipboard is not null)
                    {
                        await clipboard.SetTextAsync(result).ConfigureAwait(true);
                        message = "Bestellung zur Zwischenablage kopiert";
                    }
                    else
                    {
                        message = "Keine Zwischenablage verfügbar";
                    }
                    
                    await MessageBox.ShowAsync(App.MainWindow, message).ConfigureAwait(false);

                }).ConfigureAwait(false);
        }
        finally
        {
            end();
        }
    }
}