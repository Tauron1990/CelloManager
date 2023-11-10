using System;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using QuestPDF.Infrastructure;

namespace CelloManager.Core.Printing.Impl;

public abstract class FileSelectingDocument<TSelf> : IInternalDocument
    where TSelf : FileSelectingDocument<TSelf>, new()
{
    private IDocument? _printView;

    protected IDocument PrintView
    {
        get
        {
            if(_printView is null)
                throw new InvalidOperationException("Print Document not Initialized");

            return _printView;
        }
    }
    
    public abstract DocumentType Type { get; }

    public static IPrintDocument GenerateDocument(IDocument view)
    {
        var doc = new TSelf();
        doc.Init(view);

        return doc;
    }

    public async ValueTask Execute(Dispatcher dispatcher, Action end)
    {
        try
        {
            var saveOptions = new FilePickerSaveOptions();
            await ConfigurateDialog(saveOptions).ConfigureAwait(false);

            IStorageFile? result = await App.StorageProvider.SaveFilePickerAsync(saveOptions);
            if(result is null) return;

            await RenderTo(dispatcher, result);
        }
        finally
        {
            end();
        }
    }

    protected abstract ValueTask RenderTo(Dispatcher dispatcher, IStorageFile file);
    
    protected abstract ValueTask ConfigurateDialog(FilePickerSaveOptions dialog);

    protected virtual void Init(IDocument view) => _printView = view;

    public void Dispose() => _printView = null;
}