using System;
using System.Drawing.Printing;
using System.Linq;
using System.Reactive.Disposables;
using DynamicData;
using ReactiveUI;

namespace CelloManager.Core.Printing;

public sealed class PrintProgressManager : ReactiveObject, IDisposable
{
    private readonly SourceList<string> _printers = new();
    private readonly StandardPrintController _printController = new();
    
    private bool _isInProgress;
    private PrintDocument? _printDocument;

    public bool IsInProgress
    {
        get => _isInProgress;
        private set => this.RaiseAndSetIfChanged(ref _isInProgress, value);
    }

    public PrintDocument? PrintDocument
    {
        get => _printDocument;
        private set => this.RaiseAndSetIfChanged(ref _printDocument, value);
    }

    public PrinterSettings PrintSettings
    {
        get
        {
            if(PrintDocument is null)
                throw new InvalidOperationException("No Print Document Setted");

            return PrintDocument.PrinterSettings;
        }
    }
    
    public IObservable<IChangeSet<string>> InstalledPrinters => _printers.Connect();

    public void Init(PrintDocument document)
    {
        using (RunProgress())
        {
            PrintDocument = document;
            
            RefreshPrintersInternal();
        }
    }

    public void RefreshPrinters()
    {
        using (RunProgress())
            RefreshPrintersInternal();
    }

    private void RefreshPrintersInternal()
        => _printers.Edit(
            l =>
            {
                l.Clear();
                l.AddRange(PrinterSettings.InstalledPrinters.Cast<string>());
            });

    private IDisposable RunProgress()
    {
        IsInProgress = true;
        return Disposable.Create(this, static m => m.IsInProgress = false);
    }

    public void Dispose() => _printers.Dispose();
}