using System;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using CelloManager.Avalonia.Core.Comp;
using CelloManager.Avalonia.Core.Logic;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;

namespace CelloManager.Avalonia.ViewModels.Importing;

public sealed class ImportViewModel : ViewModelBase, ITabInfoProvider, IDisposable
{
    private readonly SpoolManager _manager;
    
    private readonly ObservableAsPropertyHelper<bool> _canClose;
    public string Title => "Importieren";

    public bool CanClose => _canClose.Value;

    public ReactiveCommand<string?, Unit> Import { get; set; }
    
    public ImportViewModel(SpoolManager manager)
    {
        _manager = manager;
        Import = ReactiveCommand.CreateFromObservable<string?, Unit>(ExecuteImport);
        _canClose = Import.IsExecuting.Select(e => !e).ToProperty(this, m => m.CanClose, scheduler: RxApp.MainThreadScheduler);
    }

    private IObservable<Unit> ExecuteImport(string? path) 
        => Observable.Return(path)
            .SelectMany(async p =>
            {
                if (!string.IsNullOrWhiteSpace(p))
                    return p;

                var browser = new OpenFolderDialog { Directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) };
                return await browser.ShowAsync(App.MainWindow);
            })
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .SelectMany(async p =>
            {
                await using var context = new CoreDatabase();
                context.Database.SetConnectionString(p);

                return await context.CelloSpools.ToArrayAsync();
            })
            .Select(spools =>
            {
                foreach (var spool in spools)
                {
                    if(_manager.Exist(spool.Name, spool.Type))
                        _manager.UpdateSpool();
                }
                
                return Unit.Default;
            });

    public void Dispose()
    {
        _canClose.Dispose();
        Import.Dispose();
    }
}