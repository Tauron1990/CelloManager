using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Threading;
using CelloManager.Core.Printing.Data;
using CelloManager.Core.Printing.Steps;

namespace CelloManager.Core.Printing.Internal;

public sealed class PrintUiModel
{
    private Queue<PrintPage> _pages = new();
    private Dispatcher? _dispatcher;
    private Window _window;
    
    public int PendingCount { get; private set; }

    public void Next()
    {
        if(_window is null || _dispatcher is null)
            throw new InvalidOperationException("Dispatcher or Window for Preview is null");
    }
    
    public void Init(PrinterContext context)
    {
        _dispatcher = context.Dispatcher;
    }
}