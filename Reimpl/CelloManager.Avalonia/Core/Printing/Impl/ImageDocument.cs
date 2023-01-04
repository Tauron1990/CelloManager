using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using SkiaSharp;

namespace CelloManager.Core.Printing.Impl;

public sealed class ImageDocument : FileSelectingDocument<ImageDocument>
{
    public override DocumentType Type => DocumentType.Image;
    
    protected override async ValueTask RenderTo(Dispatcher dispatcher, string path)
    {
        if(!path.EndsWith(".png"))
            path = $"{path}.png";


        await using var fileStream = new FileStream(path, FileMode.Create);
        
        await Dispatcher.UIThread.InvokeAsync(() => PrintView.RenderTo(fileStream));
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