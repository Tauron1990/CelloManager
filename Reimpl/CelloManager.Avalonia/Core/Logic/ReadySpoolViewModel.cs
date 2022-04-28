using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using CelloManager.Avalonia.Core.Data;
using CelloManager.Avalonia.ViewModels;
using ReactiveUI;

namespace CelloManager.Avalonia.Core.Logic;

public sealed class ReadySpoolViewModel : ViewModelBase, IDisposable
{
    private static string[] _propertys = { nameof(Category), nameof(Name), nameof(Amount), nameof(NeedAmount), nameof(NeedAmountSet) };

    private object _lock = new();
    private SpoolData _data;
    private readonly SpoolRepository _spools;

    public string Category => _data.Category;
    
    public string Name => _data.Name;
    
    public int Amount => _data.Amount;

    public int NeedAmount => _data.NeedAmount;

    public bool NeedAmountSet => _data.NeedAmount > 0;

    public ReactiveCommand<Unit, Unit> Increment { get; }

    public ReactiveCommand<Unit, Unit> Decrement { get; }
    
    public ReadySpoolViewModel(SpoolData data, SpoolRepository spools)
    {
        _data = data;
        _spools = spools;

        Increment = ReactiveCommand.CreateFromObservable(RunIncrement);
        Decrement = ReactiveCommand.CreateFromObservable(RunDecrement, this.WhenAnyValue(m => m.Amount).StartWith(data.Amount).Select(a => a > 0));
    }

    private IObservable<Unit> RunDecrement()
        => Observable.Return((Amount, _data, _spools))
            .ObserveOn(Scheduler.Default)
            .Synchronize(_lock)
            .Where(d => d.Amount > 0)
            .Select(d => d._spools.UpdateSpool(d._data with{ Amount = _data.Amount - 1}));

    private IObservable<Unit> RunIncrement()
        => Observable.Return((Amount, _data, _spools))
            .ObserveOn(Scheduler.Default)
            .Synchronize(_lock)
            .Where(d => d.Amount != int.MaxValue)
            .Select(d => d._spools.UpdateSpool(d._data with{ Amount = _data.Amount + 1}));

    public void UpdateData(SpoolData data)
    {
        foreach (var property in _propertys) this.RaisePropertyChanging(property);

        _data = data;
        
        foreach (var property in _propertys) this.RaisePropertyChanged(property);
    }
    
    
    
    public void Dispose()
    {
        Increment.Dispose();
        Decrement.Dispose();
    }
}