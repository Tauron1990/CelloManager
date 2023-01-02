using System;
using System.Drawing.Printing;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CelloManager.Avalonia.Core.Data;
using CelloManager.Avalonia.Core.Movere.ViewModels;
using CelloManager.Avalonia.Core.Movere.Views;
using CelloManager.Avalonia.Views.Orders;
using Material.Colors;
using Material.Styles.Themes;
using Material.Styles.Themes.Base;
using Image = System.Drawing.Image;
using PixelSize = Avalonia.PixelSize;
using Visual = Avalonia.Visual;

namespace CelloManager.Avalonia.Core.Movere;

public sealed class PrintBuilder
{
    public PrintDocument GenerateDocument(PendingOrder order)
    {
        var doc = new PrintDocument();
        doc.PrintPage += DocumentOnPrintPage;

        return doc;
        
        void DocumentOnPrintPage(object sender, PrintPageEventArgs e)
        {
            var window = new Window();
            var control = new PendingOrderPrintView(order);
            window.Content = control;
            
            window.Resources.MergedDictionaries
                .Add(new BundledTheme
                {
                    BaseTheme = BaseThemeMode.Light,
                    PrimaryColor = PrimaryColor.Grey,
                });

            using var stream = new MemoryStream();
            RenderTo(control, stream);
            stream.Position = 0;
            using Image? bitmap = Image.FromStream(stream);
            
            e.Graphics.DrawImage(bitmap, 10, 10);
        }
    }

    public async ValueTask ShowDialog(PrintDocument document)
    {
        bool result = false;
        Task dialogTask = Task.CompletedTask;
        
        await Dispatcher.UIThread.InvokeAsync(
            () =>
            {
                var dialog = new PrintDialog();
                var model = new PrintDialogViewModel(
                    document,
                    b =>
                    {
                        dialog.Close();
                        result = b;
                    });
                dialog.ViewModel = model;
                
                dialogTask = dialog.ShowDialog(App.MainWindow);
            });

        await dialogTask;
        
        if(result)
            document.Print();
    }

    private static void RenderTo(Control source, Stream destination, double dpi = 96)
        {
            if (source.TransformedBounds == null)
            {
                return;
            }
            Rect rect = source.TransformedBounds.Value.Clip;
            Point top = rect.TopLeft;
            var pixelSize = new PixelSize((int)rect.Width, (int)rect.Height);
            var dpiVector = new Vector(dpi, dpi);

            // get Visual root
            var root = source.GetVisualRoot() as Control ?? source;

            IDisposable? clipToBoundsSetter = default;
            IDisposable? renderTransformOriginSetter = default;
            try
            {
                // Set clip region
                var clipRegion = new RectangleGeometry(rect);
                clipToBoundsSetter = root.SetValue(Visual.ClipToBoundsProperty, true, BindingPriority.Animation);
                root.SetValue(Visual.ClipProperty, (object?)clipRegion, BindingPriority.Animation);

                // Translate origin
                renderTransformOriginSetter = root.SetValue(Visual.RenderTransformOriginProperty,
                    new RelativePoint(top, RelativeUnit.Absolute),
                    BindingPriority.Animation);

                root.SetValue(Visual.RenderTransformProperty,
                    (object?)new TranslateTransform(-top.X, -top.Y),
                    BindingPriority.Animation);

                using var bitmap = new RenderTargetBitmap(pixelSize, dpiVector);
                bitmap.Render(root);
                
                bitmap.Save(destination);
            }
            finally
            {
                renderTransformOriginSetter?.Dispose();
                clipToBoundsSetter?.Dispose();
                source.InvalidateVisual();
            }
        }
}