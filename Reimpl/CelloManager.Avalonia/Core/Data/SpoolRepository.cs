using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CelloManager.Core.Data.Converter;
using CelloManager.Data;
using DynamicData;
using DynamicData.Kernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SQLitePCL;

namespace CelloManager.Core.Data;

public sealed partial class SpoolRepository(ErrorDispatcher errorDispatcher, ILogger<SpoolRepository> logger) : IDisposable
{
    private readonly SourceCache<PendingOrder, string> _orders = new(po => po.Id);
    private readonly SourceCache<PriceDefinition, string> _priceses = new(pd => pd.Id);
    private readonly SourceCache<SpoolData, string> _spools = new(sd => sd.Id);
    private readonly CompositeDisposable _subscriptions = new();

    public IObservable<int> SpoolCount => _spools.CountChanged;

    public IEnumerable<SpoolData> SpoolItems => _spools.Items;

    public IObservable<IChangeSet<SpoolData, string>> Spools => _spools.Connect().ObserveOn(Scheduler.Default);

    public IObservable<IChangeSet<PendingOrder, string>> Orders => _orders.Connect().ObserveOn(Scheduler.Default);

    public IObservable<IChangeSet<PriceDefinition, string>> Prices => _priceses.Connect().ObserveOn(Scheduler.Default);

    public void Dispose()
    {
        _subscriptions.Dispose();
        _spools.Dispose();
        _orders.Dispose();
        _priceses.Dispose();
    }

    [LoggerMessage(Message = "Initializing Spool Repository", Level = LogLevel.Information)]
    private partial void InitRepository();

    public async Task Init()
    {
        Batteries_V2.Init();
        DataOperationManager db = DataOperationManager.Manager;

        InitRepository();

        List<SpoolData> spools = await db.Query(context => context.Spools, sel => sel.Select(d => d.FromDatabase())).ConfigureAwait(false);
        List<PendingOrder> orders = await db.Query(context => context.Orders, sel => sel.Select(d => d.FromDatabase())).ConfigureAwait(false);
        List<PriceDefinition> prices = await db.Query(
                                                    context => context.Prices,
                                                    sel => sel.Select(d => d.FromDatabase()))
                                               .ConfigureAwait(false);

        _spools.Edit(e =>
                     {
                         foreach (SpoolData spoolData in spools)
                         {
                             e.AddOrUpdate(spoolData);
                         }
                     });

        _orders.Edit(e =>
                     {
                         foreach (PendingOrder order in orders)
                         {
                             e.AddOrUpdate(order);
                         }
                     });

        _priceses.Edit(
            e =>
            {
                foreach (PriceDefinition price in prices)
                {
                    e.AddOrUpdate(price);
                }
            });

        CreateSavePipeLine(_spools, sdb => sdb.Spools, data => data.ToDatabase());
        CreateSavePipeLine(_orders, sdb => sdb.Orders, order => order.ToDatabase());
        CreateSavePipeLine(_priceses, sdb => sdb.Prices, definition => definition.ToDatabase());
    }

    private void CreateSavePipeLine<TData, TDatabase>(
        SourceCache<TData, string> cache,
        Func<SpoolDataBase, DbSet<TDatabase>> setSelector,
        Func<TData, TDatabase> converter)
        where TData : IHasId
        where TDatabase : class
    {
        IObservable<IChangeSet<TData, string>> data = cache.Connect();
        if (cache.Count != 0)
        {
            data = data.SkipInitial();
        }

        _subscriptions.Add(
            data
           .OnItemRemoved(r => EnqueueWorkItme(set => set.Remove(converter(r))))
           .OnItemAdded(a => EnqueueWorkItme(set => set.Add(converter(a))))
           .OnItemUpdated((u, _) => EnqueueWorkItme(set => set.Update(converter(u))))
           .Subscribe()
            );
        return;

        void EnqueueWorkItme(Action<DbSet<TDatabase>> op)
        {
            DatabaseWorkitem item = new(
                db =>
                {
                    DbSet<TDatabase> set = setSelector(db);
                    op(set);
                    return ValueTask.CompletedTask;
                },
                errorDispatcher.Send);
            DataOperationManager.Manager.RunItem(item);
        }
    }

    public Optional<PriceDefinition> TryFindPrice(string name)
        => _priceses.Lookup(PriceDefinition.CreateId(name));

    public Optional<SpoolData> LookUp(string name, string category)
        => _spools.Lookup(SpoolData.CreateId(name, category));

    public Unit UpdateSpool(SpoolData data)
    {
        _spools.AddOrUpdate(data);
        return Unit.Default;
    }

    public void Edit(Action<ISourceUpdater<SpoolData, string>> editor) => _spools.Edit(editor);

    public bool ValidateName([NotNullWhen(true)] string? name, [NotNullWhen(true)] string? category)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(category))
        {
            return false;
        }

        string id = SpoolData.CreateId(name, category);
        return !_spools.Keys.Contains(id, StringComparer.Ordinal);
    }

    public void AddOrder(PendingOrder order)
        => _orders.AddOrUpdate(order);

    public void Delete(SpoolData data) => _spools.Remove(data);

    public void Delete(PendingOrder order) => _orders.Remove(order);

    public void UpdatePrice(PriceDefinition? definition)
    {
        if (definition is null)
        {
            return;
        }

        _priceses.Edit(
            e =>
            {
                PriceDefinition? there = e.Items.FirstOrDefault(pd => string.Equals(pd.Id, definition.Id, StringComparison.Ordinal));
                if (there is not null)
                    // ReSharper disable CompareOfFloatsByEqualityOperator
                {
                    if (there.Price == definition.Price && there.Lenght == definition.Lenght)
                        // ReSharper restore CompareOfFloatsByEqualityOperator
                    {
                        return;
                    }
                }

                e.AddOrUpdate(definition);
            });
    }
}