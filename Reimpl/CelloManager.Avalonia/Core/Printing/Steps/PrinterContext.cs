using System;
using System.Collections.Immutable;
using System.ComponentModel.Design;
using Avalonia.Threading;
using CelloManager.Core.Data;
using CelloManager.Core.Printing.Data;
using CelloManager.Core.Printing.Internal;
using QuestPDF.Infrastructure;

namespace CelloManager.Core.Printing.Steps;

public sealed class PrinterContext(PrintUiModel printUiModel)
{
    public PrintUiModel PrintUiModel { get; } = printUiModel;

    public PendingOrder? Order { get; set; }

    public DocumentType? DocumentType { get; set; }

    public IPrintDocument? PrintDocument { get; set; }

    public IDocument? Document { get; set; }

    public Action? End { get; set; }

    public IServiceProvider ServiceProvider { get; set; } = new ServiceContainer();

    public ImmutableArray<PrintPage> Pages { get; set; } = ImmutableArray<PrintPage>.Empty;

    public Dispatcher Dispatcher
    {
        get => field ??= Dispatcher.UIThread;
        set;
    }

    public void AddPage(PrintPage page)
        => Pages = Pages.Add(page);
}