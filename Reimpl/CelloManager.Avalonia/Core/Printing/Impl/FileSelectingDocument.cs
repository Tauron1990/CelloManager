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
            var saveOptions = await ConfigurateDialog().ConfigureAwait(false);

            var result = await App.StorageProvider.SaveFilePickerAsync(saveOptions).ConfigureAwait(false);
            if(result is null) return;

            await RenderTo(dispatcher, result).ConfigureAwait(false);
        }
        finally
        {
            end();
        }
    }

    protected async ValueTask<IStorageFolder?> GetFolder(string path) 
        => await App.StorageProvider.TryGetFolderFromPathAsync(path).ConfigureAwait(false);

    protected abstract ValueTask RenderTo(Dispatcher dispatcher, IStorageFile file);
    
    protected abstract ValueTask<FilePickerSaveOptions> ConfigurateDialog();

    protected virtual void Init(IDocument view) => _printView = view;

    public void Dispose() => _printView = null;
}