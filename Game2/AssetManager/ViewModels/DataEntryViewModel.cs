using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Aggregation;
using DynamicData.Kernel;
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

    protected abstract string IndexFile { get; }

    protected DataEntryViewModel()
    {
        _entrys.Connect()
            .Scan(ImmutableDictionary<string, string>.Empty, UpdateDic)
            .SelectMany(
                dic =>
                {
                    return Observable.Return(Path.Combine(_currentPath.Value, IndexFile))
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
    }

    public void Reset() => _entrys.Clear();

    public async ValueTask Load(string path)
    {
        _currentPath.OnNext(ResolveFilePath(path));
        var targetFile = Path.Combine(_currentPath.Value, IndexFile);
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