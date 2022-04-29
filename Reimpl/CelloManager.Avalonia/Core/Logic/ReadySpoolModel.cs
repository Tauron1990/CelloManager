using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using CelloManager.Avalonia.Core.Data;

namespace CelloManager.Avalonia.Core.Logic;

public sealed class ReadySpoolModel
{ 
    private readonly object _lock = new();
    private readonly SpoolData _data;
    private readonly SpoolRepository _spools;

    public string Category => _data.Category;
    
    public string Name => _data.Name;
    
    public int Amount => _data.Amount;

    public int NeedAmount => _data.NeedAmount;

    public bool NeedAmountSet => _data.NeedAmount > 0;

    public ReadySpoolModel(SpoolData data, SpoolRepository spools)
    {
        _data = data;
        _spools = spools;
    }

    public IObservable<Unit> RunDecrement()
        => Observable.Return((Amount, _data, _spools))
            .ObserveOn(Scheduler.Default)
            .Synchronize(_lock)
            .Where(d => d.Amount > 0)
            .Select(d => d._spools.UpdateSpool(d._data with{ Amount = _data.Amount - 1}));

    public IObservable<Unit> RunIncrement()
        => Observable.Return((Amount, _data, _spools))
            .ObserveOn(Scheduler.Default)
            .Synchronize(_lock)
            .Where(d => d.Amount != int.MaxValue)
            .Select(d => d._spools.UpdateSpool(d._data with { Amount = _data.Amount + 1 }));
}