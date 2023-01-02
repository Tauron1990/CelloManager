using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using CelloManager.Avalonia.Core.Data;

namespace CelloManager.Avalonia.Core.Logic;

public sealed class ReadySpoolModel
{ 
    private readonly object _lock = new();
    private readonly SpoolRepository _spools;

    public string Category => Data.Category;
    
    public string Name => Data.Name;
    
    public int Amount => Data.Amount;

    public int NeedAmount => Data.NeedAmount;

    public bool NeedAmountSet => Data.NeedAmount > 0;
    
    public SpoolData Data { get; }

    public ReadySpoolModel(SpoolData data, SpoolRepository spools)
    {
        Data = data;
        _spools = spools;
    }

    public static implicit operator SpoolData(ReadySpoolModel model)
        => model.Data;
    
    // public IObservable<Unit> UpdateSpool(Func<SpoolData, SpoolData?> data)
    //     => Observable.Return((Data, _spools, NewData:data(Data)))
    //         .ObserveOn(Scheduler.Default)
    //         .Synchronize(_lock)
    //         .Where(t => t.Data != t.NewData && t.NewData is not null)
    //         .Select(d => d._spools.UpdateSpool(d.NewData!));

    public void RunDecrement()
        => Observable.Return((Amount, Data, _spools))
            .ObserveOn(Scheduler.Default)
            .Synchronize(_lock)
            .Where(d => d.Amount > 0)
            .Select(d => d._spools.UpdateSpool(d.Data with{ Amount = Data.Amount - 1}))
            .Subscribe();

    public void RunIncrement()
        => Observable.Return((Amount, Data, _spools))
            .ObserveOn(Scheduler.Default)
            .Synchronize(_lock)
            .Where(d => d.Amount != int.MaxValue)
            .Select(d => d._spools.UpdateSpool(d.Data with { Amount = Data.Amount + 1 }))
            .Subscribe();
}