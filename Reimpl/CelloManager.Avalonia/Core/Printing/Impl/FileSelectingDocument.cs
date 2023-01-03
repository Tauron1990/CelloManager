using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using CelloManager.Views.Orders;
using SQLitePCL;

namespace CelloManager.Core.Printing.Impl;

public abstract class FileSelectingDocument<TSelf> : IInternalDocument
    where TSelf : FileSelectingDocument<TSelf>, new()
{
    private PendingOrderPrintView? _printView;

    protected PendingOrderPrintView PrintView
    {
        get
        {
            if(_printView is null)
                throw new InvalidOperationException("Print Document not Initialized");

            return _printView;
        }
    }
    
    public abstract DocumentType Type { get; }

    public static IPrintDocument GenerateDocument(PendingOrderPrintView view)
    {
        var doc = new TSelf();
        doc.Init(view);

        return doc;
    }

    public async ValueTask Execute(Dispatcher dispatcher, Action end)
    {
        try
        {
            var diag = new SaveFileDialog();
            ConfigurateDialog(diag);

            string? result = await diag.ShowAsync(App.MainWindow);
            if(string.IsNullOrWhiteSpace(result)) return;

            await RenderTo(dispatcher, result);
        }
        finally
        {
            end();
        }
    }

    protected abstract ValueTask RenderTo(Dispatcher dispatcher, string path);
    
    protected abstract void ConfigurateDialog(SaveFileDialog dialog);

    protected virtual void Init(PendingOrderPrintView view) => _printView = view;

    public void Dispose() => _printView = null;
}