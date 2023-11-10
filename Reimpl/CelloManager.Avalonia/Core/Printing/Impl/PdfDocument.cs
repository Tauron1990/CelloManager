using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using ImageMagick;
using SkiaSharp;
using TempFileStream.Abstractions;

namespace CelloManager.Core.Printing.Impl;

public sealed class PdfDocument : FileSelectingDocument<PdfDocument>
{
    public override DocumentType Type => DocumentType.Pdf;
    protected override async ValueTask RenderTo(Dispatcher dispatcher, IStorageFile file)
    {
        if(!file.EndsWith(".pdf"))
            file = $"{file}.pdf";

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

        await collection.WriteAsync(file, MagickFormat.Pdf);
    }

    protected override ValueTask ConfigurateDialog(FilePickerSaveOptions dialog)
    {
        var filter = new FileDialogFilter
        {
            Name = "PDF",
        };
        filter.Extensions.Add("pdf");
        
        dialog.Filters!.Add(filter);
        dialog.Title = "PDF Datei";
        dialog.InitialFileName = "print.pdf";
        dialog.Directory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    }
}