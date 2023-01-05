using System.IO;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using ImageMagick;
using TempFileStream.Abstractions;

namespace CelloManager.Core.Printing.Impl;

public sealed class ImageDocument : FileSelectingDocument<ImageDocument>
{
    public override DocumentType Type => DocumentType.Image;
    
    protected override async ValueTask RenderTo(Dispatcher dispatcher, string path)
    {
        if(!path.EndsWith(".png"))
            path = $"{path}.png";


        MagickNET.Initialize();
        using var collection = new MagickImageCollection();
        using CompositeDisposable images = new();

        foreach (ITempFile imageFile in PrintView)
        {
            var image = new MagickImage();
            await image.ReadAsync(imageFile.CreateReadStream());
            
            images.Add(image);
            collection.Add(image);
        }

        using var merged = collection.AppendVertically();
        await merged.WriteAsync(path, MagickFormat.Png);
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