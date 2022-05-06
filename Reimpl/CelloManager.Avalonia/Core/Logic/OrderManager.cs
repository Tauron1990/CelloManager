using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
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

    public IObservable<IChangeSet<PendingOrder, string>> Orders { get; }
    
    public OrderManager(SpoolRepository repository)
    {
        _repository = repository;
        
        _orders = repository.Orders.AsObservableCache().DisposeWith(_disposer);
        _spools = repository.Spools.AsObservableCache().DisposeWith(_disposer);
        Orders = repository.Orders;

        CanOrder = repository.Spools.QueryWhenChanged()
            .CombineLatest(repository.Orders.QueryWhenChanged())
            .SelectMany(l => l.First.Items.Select(sd => (Data:sd, Orders:l.Second)))
            .Where(s => s.Data.NeedAmount > 0)
            .Select(s => s.Data with { Amount = s.Data.Amount + s.Orders.Items.SelectMany(o => o.Spools).Where(po => po.SpoolId == s.Data.Id).Sum(po => po.Amount) })
            .Where(s => s.Amount < s.NeedAmount)
            .Count()
            .Select(c => c > 0);
    }

    public bool PlaceOrder()
    {
        var toOrder = _spools.Items
            .Select(sd => sd with { Amount = sd.Amount + _orders.Items.SelectMany(o => o.Spools).Where(os => os.SpoolId == sd.Id).Sum(os => os.Amount) })
            .Where(sd => sd.Amount < sd.NeedAmount)
            .Select(sd => new OrderedSpool(sd.Id, sd.NeedAmount - sd.Amount))
            .ToImmutableList();
        
        if(toOrder.Count == 0) return false;
        
        _repository.AddOrder(PendingOrder.New(toOrder));
        
        return true;
    }
    
    void IDisposable.Dispose() => _disposer.Dispose();

    public void CompledOrder(PendingOrder order)
    {
        _repository.Delete(order);

        foreach (var orderSpool in order.Spools)
        {
            var data = _spools.Lookup(orderSpool.SpoolId);

            if (data.HasValue)
                _repository.UpdateSpool(data.Value with { Amount = data.Value.Amount + orderSpool.Amount });
        }
    }
}