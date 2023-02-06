using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using DynamicData;
using DynamicData.Kernel;
using Microsoft.Extensions.Logging;

namespace CelloManager.Core.Data;

public sealed partial class SpoolRepository : IDisposable
{
    private readonly SourceCache<SpoolData, string> _spools = new(sd => sd.Id);
    private readonly SourceCache<PendingOrder, string> _orders = new(po => po.Id);
    private readonly SourceCache<PriceDefinition, string> _priceses = new(pd => pd.Id);

    private readonly ErrorDispatcher _errorDispatcher;
    private readonly ILogger<SpoolRepository> _logger;
    private readonly IBlobCache _blobCache = BlobCache.UserAccount;
    private readonly CompositeDisposable _subscriptions = new();

    public IObservable<int> SpoolCount => _spools.CountChanged;

    public SpoolRepository(ErrorDispatcher errorDispatcher, ILogger<SpoolRepository> logger)
    {
        _errorDispatcher = errorDispatcher;
        _logger = logger;
    }

    [LoggerMessage(Message = "Initializing Spool Repository", Level = LogLevel.Information)]
    private partial void InitRepository();
    
    public async Task Init()
    {
        InitRepository();
        
        var spools = await _blobCache.GetAllObjects<SpoolData>();
        var orders = await _blobCache.GetAllObjects<PendingOrder>();
        var prices = await _blobCache.GetAllObjects<PriceDefinition>();
        
        _spools.Edit(e =>
        {
            foreach (SpoolData spoolData in spools) e.AddOrUpdate(spoolData);
        });
        
        _orders.Edit(e =>
        {
            foreach (PendingOrder order in orders) e.AddOrUpdate(order);
        });

        _priceses.Edit(
            e =>
            {
                foreach (PriceDefinition price in prices) e.AddOrUpdate(price);
            });
        
        StartVacoumTimer();
        
        CreateSavePipeLine(_spools);
        CreateSavePipeLine(_orders);
        CreateSavePipeLine(_priceses);
    }

    private void CreateSavePipeLine<TData>(SourceCache<TData, string> cache)
        where TData : IHasId
    {
        var data = cache.Connect();
        if(cache.Count != 0)
            data = data.SkipInitial();

        data
            .OnItemRemoved(r => ReportError(_blobCache.Invalidate(r.Id)))
            .OnItemAdded(a => ReportError(_blobCache.InsertObject(a.Id, a)))
            .OnItemUpdated((u, _) => ReportError(_blobCache.InsertObject(u.Id, u)))
            .Subscribe().DisposeWith(_subscriptions);
    }

    private void StartVacoumTimer()
    {
        Observable.Timer(DateTimeOffset.Now, TimeSpan.FromHours(24))
            .SelectMany(_ => _blobCache.Vacuum().OnErrorResumeNext(Observable.Empty<Unit>()))
            .Subscribe().DisposeWith(_subscriptions);
    }

    private void ReportError(IObservable<Unit> toReport) => toReport.Subscribe(_ => { }, e => _errorDispatcher.Send(e));

    public IEnumerable<SpoolData> SpoolItems => _spools.Items;

    public IObservable<IChangeSet<SpoolData, string>> Spools => _spools.Connect().ObserveOn(Scheduler.Default);

    public IObservable<IChangeSet<PendingOrder, string>> Orders => _orders.Connect().ObserveOn(Scheduler.Default);

    public IObservable<IChangeSet<PriceDefinition, string>> Prices => _priceses.Connect().ObserveOn(Scheduler.Default);

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

    public bool ValidateName([NotNullWhen(true)]string? name, [NotNullWhen(true)]string? category)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(category))
            return false;
        
        var id = SpoolData.CreateId(name, category);
        return !_spools.Keys.Contains(id);
    }

    public void AddOrder(PendingOrder order)
        => _orders.AddOrUpdate(order);
    
    public void Dispose()
    {
        _subscriptions.Dispose();
        _spools.Dispose();
        _orders.Dispose();
        _priceses.Dispose();
    }

    public void Delete(SpoolData data) => _spools.Remove(data);

    public void Delete(PendingOrder order) => _orders.Remove(order);

    public void UpdatePrice(PriceDefinition? definition)
    {
       if(definition is null) return;

       _priceses.Edit(
           e =>
           {
               var there = e.Items.FirstOrDefault(pd => pd.Id == definition.Id);
               if(there is not null)
                   if(there.Price == definition.Price && there.Lenght == definition.Lenght)
                       return;
               
               e.AddOrUpdate(definition);
           });
    }
}