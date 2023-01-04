using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Rendering;
using Avalonia.Skia;
using Avalonia.Skia.Helpers;
using Avalonia.VisualTree;
using SkiaSharp;

namespace CelloManager.Core.Printing.Impl;

public static class RenderExtensions
{
    public static void RenderTo(this Control control, SKDocument document)
    {
        IRenderRoot source = control.GetVisualRoot();
        Rect bounds = source.Bounds;
        SKCanvas? page = document.BeginPage((float)bounds.Width, (float)bounds.Height);
        
        using var context = new DrawingContext(DrawingContextHelper.WrapSkiaCanvas(page, SkiaPlatform.DefaultDpi));
        ImmediateRenderer.Render(source.GetVisualRoot(), context);

        document.EndPage();
    }
    
    public static void RenderTo(this Control source, SKCanvas canvas)
    {
        using var context = new DrawingContext(DrawingContextHelper.WrapSkiaCanvas(canvas, SkiaPlatform.DefaultDpi));
        ImmediateRenderer.Render(source.GetVisualRoot(), context);
    }

    public static void RenderTo(this Control source, Stream destination)
    {
        using var bitmap = new RenderTargetBitmap(new PixelSize((int)source.Bounds.Width, (int)source.Bounds.Height), SkiaPlatform.DefaultDpi);
        bitmap.Render(source.GetVisualRoot());

        bitmap.Save(destination);
    }
    
    /*public static void RenderTo(this Control source, Stream destination, Vector? dpi = null)
    {
        if(source.TransformedBounds == null)
        {
            return;
        }
        Rect rect = source.TransformedBounds.Value.Clip;
        Point top = rect.TopLeft;
        var pixelSize = new PixelSize((int)rect.Width, (int)rect.Height);
        Vector dpiVector = dpi ?? SkiaPlatform.DefaultDpi;


        IDisposable? clipToBoundsSetter = default;
        IDisposable? renderTransformOriginSetter = default;
        try
        {
            // Set clip region
            var clipRegion = new RectangleGeometry(rect);
            clipToBoundsSetter = source.SetValue(Visual.ClipToBoundsProperty, true, BindingPriority.Animation);
            source.SetValue(Visual.ClipProperty, (object?)clipRegion, BindingPriority.Animation);

            // Translate origin
            renderTransformOriginSetter = source.SetValue(
                Visual.RenderTransformOriginProperty,
                new RelativePoint(top, RelativeUnit.Absolute),
                BindingPriority.Animation);

            source.SetValue(
                Visual.RenderTransformProperty,
                (object?)new TranslateTransform(-top.X, -top.Y),
                BindingPriority.Animation);

            using var bitmap = new RenderTargetBitmap(pixelSize, dpiVector);
            bitmap.Render(source);

            bitmap.Save(destination);
        }
        finally
        {
            renderTransformOriginSetter?.Dispose();
            clipToBoundsSetter?.Dispose();
            source.InvalidateVisual();
        }
    }*/
}