using System;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CelloManager.Core.Printing.Steps;
using obo.MailtoBuilder;

namespace CelloManager.Core.Printing.Impl;

public sealed class MailDocument(PrinterContext context) : IInternalDocument
{
    public static MailDocument GenerateDocument(PrinterContext context) 
        => new(context);

    public void Dispose()
    {
        
    }

    public DocumentType Type => DocumentType.Mail;

    public async ValueTask Execute(Dispatcher dispatcher, Action end)
    {
        try
        {
            string result = TextGenerator.RenderOrder(context);
            MailtoBuilder builder = MailtoBuilder.Create();
            
            foreach (ReadOnlySpan<char> line in result.EnumerateLines())
            {
                builder.AppendBodyLine(line.ToString());
            }
            
            await dispatcher.InvokeAsync(
                async () =>
                {
                    ILauncher launer = App.MainWindow.Launcher;
                    await launer.LaunchUriAsync(builder.ToUri()).ConfigureAwait(true);
                }).ConfigureAwait(false);
        }
        finally
        {
            end();
        }
    }
}