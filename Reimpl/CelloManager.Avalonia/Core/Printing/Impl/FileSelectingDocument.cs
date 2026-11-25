using System;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CelloManager.Core.Printing.Steps;
using CelloManager.Views;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;

namespace CelloManager.Core.Printing.Impl;

public abstract class FileSelectingDocument<TSelf> : IInternalDocument
    where TSelf : FileSelectingDocument<TSelf>, new()
{
    private IDocument? _printView;
    private IServiceProvider? _serviceProvider;

    private IServiceProvider ServiceProvider => _serviceProvider ?? throw new InvalidOperationException("Service Provider not Initialized");
    private IStorageProvider StorageProvider =>  ServiceProvider.GetRequiredService<MainWindowProvider>().StorageProvider;
    
    protected IDocument PrintView => _printView ?? throw new InvalidOperationException("Print Document not Initialized");
    
    public abstract DocumentType Type { get; }

    public static IPrintDocument GenerateDocument(IDocument view, PrinterContext context)
    {
        TSelf doc = new();
        doc.Init(view, context);

        return doc;
    }

    public async ValueTask Execute(Dispatcher dispatcher, Action end)
    {
        try
        {
            FilePickerSaveOptions saveOptions = await ConfigurateDialog().ConfigureAwait(false);

            IStorageFile? result = await StorageProvider.SaveFilePickerAsync(saveOptions).ConfigureAwait(false);
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
        => await StorageProvider.TryGetFolderFromPathAsync(path).ConfigureAwait(false);

    protected abstract ValueTask RenderTo(Dispatcher dispatcher, IStorageFile file);

    protected abstract ValueTask<FilePickerSaveOptions> ConfigurateDialog();

    protected virtual void Init(IDocument view, PrinterContext context)
    {
        _serviceProvider = context.ServiceProvider;
        _printView = view;
    }
}