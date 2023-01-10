using System;
using System.Drawing.Printing;
using Avalonia.Media.Imaging;

namespace CelloManager.ViewModels.Printing;

public sealed class ImagePreViewModel : IDisposable
{
    private readonly PreviewPageInfo _previewPageInfo;
    private readonly Lazy<IBitmap> _image;

    public ImagePreViewModel(PreviewPageInfo previewPageInfo)
    {
        _previewPageInfo = previewPageInfo;
        _image = new Lazy<IBitmap>(() => _previewPageInfo.Image.ConvertToAvaloniaBitmap());
    }

    public IBitmap Image => _image.Value;
    
    public void Dispose() => _previewPageInfo.Image.Dispose();
}