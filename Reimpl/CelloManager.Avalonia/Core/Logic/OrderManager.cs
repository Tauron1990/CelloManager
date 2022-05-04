using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CelloManager.Avalonia.Core.Data;
using DynamicData;
using DynamicData.Aggregation;
using DynamicData.Alias;

namespace CelloManager.Avalonia.Core.Logic;

public class OrderManager : IDisposable
{
    private readonly SpoolRepository _repository;
    private readonly IObservableCache<PendingOrder, string> _orders;
    private readonly IObservableCache<SpoolData, string> _spools;

    private readonly CompositeDisposable _disposer = new();

    public IObservable<bool> CanOrder { get; }

    public OrderManager(SpoolRepository repository)
    {
        _repository = repository;
        
        _orders = repository.Orders.AsObservableCache().DisposeWith(_disposer);
        _spools = repository.Spools.AsObservableCache().DisposeWith(_disposer);
        
        CanOrder = repository.Spools
            .Where(s => s.NeedAmount > 0)
            .AutoRefreshOnObservable(_ => _orders.CountChanged)
            .Select(s => s with { Amount = s.Amount + _orders.Items.SelectMany(o => o.Spools).Where(po => po.SpoolId == s.Id).Sum(po => po.Amount) })
            .Where(s => s.Amount < s.NeedAmount)
            .Count()
            .Select(c => c > 0);
    }

    public void PlaceOrder()
    {
        var toOrder = _spools.Items
            .Select(sd => sd with { Amount = _orders.Items.SelectMany(o => o.Spools).Where(os => os.SpoolId == sd.Id).Sum(os => os.Amount) })
            .Where(sd => sd.Amount < sd.NeedAmount)
            .Select(sd => new OrderedSpool(sd.Id, sd.NeedAmount - sd.Amount))
            .ToImmutableList();
        
        _repository.AddOrder(PendingOrder.New(toOrder));
    }
    
    void IDisposable.Dispose() => _disposer.Dispose();
}