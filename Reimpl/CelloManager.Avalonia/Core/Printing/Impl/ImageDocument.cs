using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CelloManager.Views;
using QuestPDF.Fluent;

namespace CelloManager.Core.Printing.Impl;

public sealed class ImageDocument : FileSelectingDocument<ImageDocument>
{
    public override DocumentType Type => DocumentType.Image;
    
    protected override async ValueTask RenderTo(Dispatcher dispatcher, IStorageFile file)
    {
        var images = new List<Bitmap>();
        
        try
        {
            var imageBytes = PrintView.GenerateImages();

            images.AddRange(imageBytes.Select(imageByte => new Bitmap(new MemoryStream(imageByte))));
            
            var newWidth = images.Select(i => i.Size).Max(size => size.Width);
            var newHeight = images.Select(i => i.Size).Sum(size => size.Height);

            var pixelSize = new PixelSize((int)newWidth, (int)newHeight);
            using var newImage =
                new WriteableBitmap(pixelSize, new Vector(288, 288));

            var offset = 0;

            using RenderTargetBitmap bitmap = new(pixelSize, new Vector(288, 288));
            using var ctx = bitmap.CreateDrawingContext();

            foreach (var image in images)
            {
                ctx.DrawImage(image, new Rect(new Point(offset, 0), image.Size));

                offset += (int)image.Size.Height;
            }

            await using var stream = await file.OpenWriteAsync();
            bitmap.Save(stream);
        }
        finally
        {
            foreach (var image in images) image.Dispose();
        }
    }

    protected override ValueTask ConfigurateDialog(FilePickerSaveOptions dialog)
    {
        dialog.Title = "Bild Datei";

        var filter = new FilePickerFileType("Bild")
        {
            Patterns = new[] { "png" }
        };

        dialog.SuggestedFileName = "print.png";
        
        
        dialog.Directory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    }
}