using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Avalonia.Controls;
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
    
    public ReactiveCommand<string?, Exception?> Import { get; set; }
    
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
            .SelectMany(async p =>
            {
                if (!string.IsNullOrWhiteSpace(p))
                    return new[] { p };

                var browser = new OpenFileDialog { Directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) };
                return await browser.ShowAsync(App.MainWindow);
            })
            .SelectMany(l => l ?? Array.Empty<string>())
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .ObserveOn(Scheduler.Default)
            .SelectMany(_manager.ImportSpools!);

    public void Dispose()
    {
        _error.Dispose();
        _isRunning.Dispose();
        _canClose.Dispose();
        Import.Dispose();
    }
}