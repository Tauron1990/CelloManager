using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CelloManager.Core.Data;
using DynamicData;
using DynamicData.Kernel;
using Microsoft.Extensions.Logging;

namespace CelloManager.Core.Logic;

public class OrderManager : IDisposable
{
    private readonly CompositeDisposable _disposer = new();
    private readonly ILogger<OrderManager> _logger;
    private readonly IObservableCache<PendingOrder, string> _orders;
    private readonly SpoolRepository _repository;
    private readonly IObservableCache<SpoolData, string> _spools;

    public OrderManager(SpoolRepository repository, ILogger<OrderManager> logger)
    {
        _repository = repository;
        _logger = logger;

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

    public IObservable<bool> CanOrder { get; }

    public IObservable<IChangeSet<PendingOrder, string>> Orders { get; }

    void IDisposable.Dispose() => _disposer.Dispose();

    private bool ValidateCanOrder((IEnumerable<SpoolData>, ImmutableList<PendingOrder>) arg)
    {
        (IEnumerable<SpoolData> data, ImmutableList<PendingOrder> orders) = arg;

        SpoolData Selector(SpoolData spoolData)
            => orders.SelectMany(o => o.Spools)
                     .SelectMany(l => l.Spools)
                     .Where(os => string.Equals(os.SpoolId, spoolData.Id, StringComparison.Ordinal))
                     .Aggregate(
                          spoolData,
                          (spool, order) => spool with { Amount = spool.Amount + order.Amount });

        return data
              .Select(Selector)
              .Any(actualData => actualData.NeedAmount > actualData.Amount);
    }

    public PendingOrder GetAll()
        => PendingOrder.New(_spools.Items, data => data.Amount);

    public bool PlaceOrder()
    {
        ImmutableList<SpoolData> toOrder = _spools.Items
                                                  .OrderBy(sd => sd.Id, StringComparer.Ordinal)
                                                  .Select(
                                                       sd => sd with
                                                             {
                                                                 Amount = sd.Amount + _orders.Items
                                                                                             .SelectMany(o => o.Spools)
                                                                                             .SelectMany(l => l.Spools)
                                                                                             .Where(os => string.Equals(os.SpoolId, sd.Id, StringComparison.Ordinal)).Sum(os => os.Amount),
                                                             })
                                                  .Where(sd => sd.Amount < sd.NeedAmount)
                                                  .ToImmutableList();

        if (toOrder.Count == 0)
        {
            _logger.LogDebug(new EventId(3001, "OrderNotPlaced"), "No order was placed because no inventory items require ordering");
            return false;
        }

        _repository.AddOrder(PendingOrder.New(toOrder, sd => sd.NeedAmount - sd.Amount));
        _logger.LogInformation(new EventId(3000, "OrderPlaced"), "Order added to the order cache for {SpoolCount} spools", toOrder.Count);

        return true;
    }

    public void CompledOrder(PendingOrder order)
    {
        _repository.Delete(order);

        foreach (OrderedSpool orderSpool in order.Spools.SelectMany(l => l.Spools))
        {
            Optional<SpoolData> data = _spools.Lookup(orderSpool.SpoolId);

            if (data.HasValue)
            {
                _repository.UpdateSpool(data.Value with { Amount = data.Value.Amount + orderSpool.Amount });
            }
        }

        _logger.LogInformation(new EventId(3002, "OrderCompleted"), "Order completed in inventory cache for {SpoolCount} spools", order.Spools.SelectMany(l => l.Spools).Count());
    }
}