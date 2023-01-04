using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using SkiaSharp;

namespace CelloManager.Core.Printing.Impl;

public sealed class PdfDocument : FileSelectingDocument<PdfDocument>
{
    public override DocumentType Type => DocumentType.PDF;
    protected override async ValueTask RenderTo(Dispatcher dispatcher, string path)
    {
        if(!path.EndsWith(".pdf"))
            path = $"{path}.pdf";

        using var doc = SKDocument.CreatePdf(path);
        
        await Dispatcher.UIThread.InvokeAsync(() => PrintView.RenderTo(doc));
    }

    protected override void ConfigurateDialog(SaveFileDialog dialog)
    {
        var filter = new FileDialogFilter
        {
            Name = "PDF",
        };
        filter.Extensions.Add("pdf");
        
        dialog.Filters!.Add(filter);
        dialog.Title = "PDF Datei";
        dialog.InitialFileName = "print.pdf";
    }
}