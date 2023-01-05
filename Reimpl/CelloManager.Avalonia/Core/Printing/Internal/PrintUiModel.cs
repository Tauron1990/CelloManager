using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using CelloManager.Core.Data;
using CelloManager.Core.Printing.Data;
using CelloManager.Core.Printing.Steps;
using CelloManager.Views.Orders;

namespace CelloManager.Core.Printing.Internal;

public sealed class PrintUiModel : IDisposable
{
    private readonly Queue<PrintPage> _pages = new();
    private Dispatcher? _dispatcher;
    private Window? _window;

    public int PendingCount => _pages.Count;

    public async ValueTask<PendingOrderPrintView> Next()
    {
        if(_window is null || _dispatcher is null)
            throw new InvalidOperationException("Dispatcher or Window for Preview is null");

        PendingOrder order = _pages.Dequeue().SeperatedOrder;
        
        return await _dispatcher.InvokeAsync(
            () =>
            {
                var control = new PendingOrderPrintView();
                control.Init(order);
                _window.Content = control;

                return control;
            });
    }
    
    public async ValueTask Init(PrinterContext context)
    {
        _dispatcher = context.Dispatcher;
        foreach (PrintPage printPage in context.Pages)
            _pages.Enqueue(printPage);

        await _dispatcher.InvokeAsync(
            () =>
            {
                _window = new Window
                {
                    SizeToContent = SizeToContent.WidthAndHeight,
                    Background = Brushes.White,
                    Foreground = Brushes.Black,
                    Title = "Blid Vorschau",
                    CanResize = false,
                    ShowInTaskbar = false,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                };

                _window.Show(App.MainWindow);
            });
    }

    public void Dispose()
    {
        _dispatcher?.Post(() =>
        {
            _window?.Close();
            _dispatcher = null;
            _window = null;
        });
    }
}