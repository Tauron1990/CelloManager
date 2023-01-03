using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using SkiaSharp;

namespace CelloManager.Core.Printing.Impl;

public sealed class ImageDocument : FileSelectingDocument<ImageDocument>
{
    public override DocumentType Type => DocumentType.Image;
    
    protected override ValueTask RenderTo(Dispatcher dispatcher, string path)
    {
        if(!path.EndsWith(".png"))
            path = $"{path}.png";

        using var image = new SKBitmap();
        using var canvas = new SKCanvas(image);

        PrintView.RenderTo(canvas);
        return ValueTask.CompletedTask;
    }

    protected override void ConfigurateDialog(SaveFileDialog dialog)
    {
        var filter = new FileDialogFilter
        {
            Name = "Bild",
        };
        filter.Extensions.Add("png");
        
        dialog.Filters!.Add(filter);
        dialog.Title = "Bild Datei";
        dialog.InitialFileName = "print.png";
    }
}