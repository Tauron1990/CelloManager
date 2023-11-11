using System;
using System.Collections.Immutable;
using System.ComponentModel.Design;
using Avalonia.Threading;
using CelloManager.Core.Data;
using CelloManager.Core.Printing.Data;
using CelloManager.Core.Printing.Internal;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;

namespace CelloManager.Core.Printing.Steps;

public sealed class PrinterContext
{
    private Dispatcher? _dispatcher;
    
    public PrintUiModel PrintUiModel { get; }
    
    public PendingOrder? Order { get; set; }

    public DocumentType? DocumentType { get; set; }

    public IPrintDocument? PrintDocument { get; set; }

    public IDocument? Document { get; set; }
    
    public Action? End { get; set; }
    
    public IServiceProvider ServiceProvider { get; set; } = new ServiceContainer();

    public ImmutableArray<PrintPage> Pages { get; set; } = ImmutableArray<PrintPage>.Empty;
    
    public Dispatcher Dispatcher
    {
        get => _dispatcher ??= Dispatcher.UIThread;
        set => _dispatcher = value;
    }

    public PrinterContext(PrintUiModel printUiModel) => PrintUiModel = printUiModel;

    public void AddPage(PrintPage page)
        => Pages = Pages.Add(page);
}