using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CelloManager.Core.Data;
using DynamicData;

namespace CelloManager.Core.Logic;

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

        CanOrder = repository.Spools.QueryWhenChanged().Select(q => q.Items).StartWith(Array.Empty<SpoolData>())
            .CombineLatest
            (
                repository.Orders.QueryWhenChanged().Select(q => q.Items.ToImmutableList()).StartWith(ImmutableList<PendingOrder>.Empty)
            )
            .Select(ValidateCanOrder);
    }

    private bool ValidateCanOrder((IEnumerable<SpoolData>, ImmutableList<PendingOrder>) arg)
    {
        var (data, orders) = arg;

        return data.Select(spoolData => orders.SelectMany(o => o.Spools)
                .Where(os => os.SpoolId == spoolData.Id)
                .Aggregate(spoolData, (spool, order) => spool with { Amount = spool.Amount + order.Amount }))
            .Any(actualData => actualData.NeedAmount > actualData.Amount);
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