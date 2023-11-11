using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Avalonia.Platform.Storage;
using CelloManager.Core.Logic;
using ReactiveUI;

namespace CelloManager.ViewModels.Importing;

public sealed class ImportViewModel : ViewModelBase, ITabInfoProvider, IDisposable
{
    private readonly SpoolManager _manager;
    
    private readonly ObservableAsPropertyHelper<bool> _canClose;
    private readonly ObservableAsPropertyHelper<bool> _isRunning;
    private readonly ObservableAsPropertyHelper<string?> _error;

    public string Title => "Importieren";

    public bool CanClose => _canClose.Value;

    public bool IsRunning => _isRunning.Value;

    public string? Error => _error.Value;
    
    public ReactiveCommand<string?, Exception?> Import { get; }
    
    public ImportViewModel(SpoolManager manager)
    {
        _manager = manager;
        Import = ReactiveCommand.CreateFromObservable<string?, Exception?>(ExecuteImport);

        _error = Import.Select(e => e?.Message).ToProperty(this, m => m.Error, scheduler: RxApp.MainThreadScheduler);
        _canClose = Import.IsExecuting.Select(e => !e).ToProperty(this, m => m.CanClose, scheduler: RxApp.MainThreadScheduler);
        _isRunning = Import.IsExecuting.ToProperty(this, m => m.IsRunning, scheduler: RxApp.MainThreadScheduler);
    }

    private IObservable<Exception?> ExecuteImport(string? path)
        => Observable.Return(path)
                     .SelectMany<string?, IStorageFile?>(
                          async p =>
                          {
                              if (!string.IsNullOrWhiteSpace(p))
                              {
                                  var provided = await App.StorageProvider
                                                          .TryGetFileFromPathAsync(p)
                                                          .ConfigureAwait(false);
                                  if(provided is not null)
                                    return provided;
                              }

                              var folder = await App.StorageProvider
                                                    .TryGetFolderFromPathAsync(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))
                                                    .ConfigureAwait(false);

                              var list = await App.StorageProvider
                                                  .OpenFilePickerAsync(new FilePickerOpenOptions { SuggestedStartLocation = folder })
                                                  .ConfigureAwait(false);

                              #pragma warning disable CA1826
                              return list.FirstOrDefault();
                              #pragma warning restore CA1826
                          })
                     .Where(sel => sel is not null)
                     .ObserveOn(Scheduler.Default)
                     .SelectMany(s => s!.Name.EndsWith(".json", StringComparison.Ordinal) 
                                     ? _manager.ImportFromJson(s) 
                                     : _manager.ImportFromLegacy(s));

    public void Dispose()
    {
        _error.Dispose();
        _isRunning.Dispose();
        _canClose.Dispose();
        Import.Dispose();
    }
}