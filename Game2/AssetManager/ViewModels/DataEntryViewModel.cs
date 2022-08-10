using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Avalonia.Controls;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Kernel;
using JetBrains.Annotations;
using Material.Dialog;
using Material.Dialog.Icons;
using Material.Dialog.Interfaces;
using Newtonsoft.Json;
using ReactiveUI;

namespace AssetManager.ViewModels;

public delegate void RenameEntry(string oldName, string newName);

public sealed class DataEntry : ViewModelBase
{
    private readonly RenameEntry _renameEntry;
    private string _name = string.Empty;
    private string _content = string.Empty;

    public string Name
    {
        get => _name;
        set
        {
            var old = _name;
            this.RaiseAndSetIfChanged(ref _name, value);
            _renameEntry(old, value);
        }
    }

    public string Content
    {
        get => _content;
        set => this.RaiseAndSetIfChanged(ref _content, value);
    }

    public DataEntry(string name, string content, RenameEntry renameEntry)
    {
        _renameEntry = renameEntry;
        Name = name;
        Content = content;
    }
}

public abstract class DataEntryViewModel : ViewModelBase, IDisposable
{
    private readonly CompositeDisposable _disposable = new();
    private readonly BehaviorSubject<string> _currentPath = new(string.Empty);
    private readonly SourceCache<DataEntry, string> _entrys = new(e => e.Name);
    private DataEntry? _currentEntry;

    public ReactiveCommand<Unit, DataEntry?> AddItem { get; }

    public ReactiveCommand<Unit, Unit> RemoveItem { get; }

    public ReactiveCommand<Unit, Unit> EditItem { get; }
    
    public IObservableList<DataEntry> Entrys { get; }

    public DataEntry? CurrentEntry
    {
        get => _currentEntry;
        [UsedImplicitly]
        set => this.RaiseAndSetIfChanged(ref _currentEntry, value);
    }

    protected DataEntryViewModel()
    {
        _entrys.Connect()
            .ObservOnDispatcher()
            .BindToObservableList(out var list)
            .Subscribe()
            .DisposeWith(_disposable);

        Entrys = list;

        AddItem = ReactiveCommand.CreateFromObservable<Unit, DataEntry?>(
                _ =>
                (
                    from dicPath in GetRootDic()
                    from entry in AddEntry(dicPath)
                    select entry
                ).CatchAndDisplayError(),
                from basePath in _currentPath
                select File.Exists(basePath))
            .DisposeWith(_disposable);

        AddItem.NotNull().Subscribe(de => _entrys.AddOrUpdate(de)).DisposeWith(_disposable);

        RemoveItem = ReactiveCommand.CreateFromObservable(
            () =>
            (
                from root in GetRootDic()
                where CurrentEntry is not null
                from result in CreateDialog().ShowDialog(App.MainWindow)
                where result.GetResult == bool.TrueString
                from del in InternalDeleteEntry(root, CurrentEntry)
                select del
            ).CatchAndDisplayError(),
            from ent in this.WhenAny(m => m.CurrentEntry, c => c.Value)
            select ent is not null);

        EditItem = ReactiveCommand.CreateFromObservable(
            () =>
            (
                from root in GetRootDic()
                where CurrentEntry is not null
                from res in EditContent(root, CurrentEntry)
                select res
            ).CatchAndDisplayError(),
            from ent in this.WhenAny(m => m.CurrentEntry, c => c.Value)
            select ent is not null);
        
        _entrys.Connect()
            .Scan(ImmutableDictionary<string, string>.Empty, UpdateDic)
            .SelectMany(
                dic =>
                {
                    return _currentPath
                        .Take(1)
                        .SelectMany(
                            async path =>
                            {
                                await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(dic));
                                return Unit.Default;
                            })
                        .CatchAndDisplayError();
                })
            .Subscribe()
            .DisposeWith(_disposable);

        ImmutableDictionary<string, string> UpdateDic(ImmutableDictionary<string, string> data, IChangeSet<DataEntry, string> changes) =>
            changes.Aggregate(data,
                (dictionary, change) => change.Reason switch
                {
                    ChangeReason.Add => dictionary.Add(change.Key, change.Current.Content),
                    ChangeReason.Remove => dictionary.Remove(change.Key),
                    _ => dictionary
                });

        IObservable<string> GetRootDic()
            => from path in _currentPath.Take(1)
                let dic = Path.GetDirectoryName(path)
                where !string.IsNullOrEmpty(dic)
                select dic;
        
        IDialogWindow<DialogResult> CreateDialog()
            => DialogHelper.CreateAlertDialog
            (
                new AlertDialogBuilderParams
                {
                    ContentHeader = "Confirm action",
                    SupportingText = "Are you sure to DELETE 20 FILES?",
                    StartupLocation = WindowStartupLocation.CenterOwner,
                    NegativeResult = new DialogResult(bool.FalseString),
                    DialogHeaderIcon = DialogIconKind.Warning,
                    DialogButtons = new[]
                    {
                        new DialogResultButton
                        {
                            Content = "Abbrechen",
                            Result = bool.FalseString
                        },
                        new DialogResultButton
                        {
                            Content = "Löschen",
                            Result = bool.TrueString
                        }
                    }
                }
            );

        async Task<Unit> InternalDeleteEntry(string root, DataEntry entry)
        {
            _entrys.Remove(entry.Name);
            await DeleteEntry(root, entry);
            return Unit.Default;
        }
    }

    protected abstract Task<DataEntry> AddEntry(string rootDic);

    protected abstract Task<Unit> EditContent(string rootDic, DataEntry entry);

    protected abstract Task<Unit> DeleteEntry(string rootPath, DataEntry dataEntry);
    
    public void Reset()
    {
        CurrentEntry = null;
        _entrys.Clear();
    }

    public async ValueTask Load(string path)
    {
        var targetFile = ResolveFilePath(path);
        _currentPath.OnNext(targetFile);
        if(!File.Exists(targetFile))
            return;
        
        var file = JsonConvert.DeserializeObject<ImmutableDictionary<string, string>>(await File.ReadAllTextAsync(targetFile));
        if(file is null) return;

        _entrys.Clear();
        _entrys.AddOrUpdate(file.Select(p => new DataEntry(p.Key, p.Value, RenameEntry)));
    }

    private void RenameEntry(string oldname, string newname)
    {
        try
        {
            _entrys.Lookup(oldname)
                .Convert(
                    e =>
                    {
                        _entrys.Remove(e);
                        return e;
                    })
                .IfHasValue(e => _entrys.AddOrUpdate(e));
        }
        catch (Exception e)
        {
            e.DisplayError();
        }
    }

    protected abstract string ResolveFilePath(string root);
    
    public void Dispose()
    {
        _disposable.Dispose();
        _entrys.Dispose();
        
        GC.SuppressFinalize(this);
    }
}