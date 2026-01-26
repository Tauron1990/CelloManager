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

    protected IDocument PrintView => _printView ?? throw new InvalidOperationException("Print Document not Initialized");

    public abstract DocumentType Type { get; }

    public static IPrintDocument GenerateDocument(IDocument view)
    {
        TSelf doc = new();
        doc.Init(view);

        return doc;
    }

    public async ValueTask Execute(Dispatcher dispatcher, Action end)
    {
        try
        {
            FilePickerSaveOptions saveOptions = await ConfigurateDialog().ConfigureAwait(false);

            IStorageFile? result = await App.StorageProvider.SaveFilePickerAsync(saveOptions).ConfigureAwait(false);
            if (result is null)
            {
                return;
            }

            await RenderTo(dispatcher, result).ConfigureAwait(false);
        }
        finally
        {
            end();
        }
    }

    public void Dispose() => _printView = null;

    protected async ValueTask<IStorageFolder?> GetFolder(string path)
        => await App.StorageProvider.TryGetFolderFromPathAsync(path).ConfigureAwait(false);

    protected abstract ValueTask RenderTo(Dispatcher dispatcher, IStorageFile file);

    protected abstract ValueTask<FilePickerSaveOptions> ConfigurateDialog();

    protected virtual void Init(IDocument view) => _printView = view;
}