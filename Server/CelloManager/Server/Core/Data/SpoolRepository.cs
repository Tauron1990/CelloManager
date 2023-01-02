using System;
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

namespace CelloManager.Avalonia.Core.Data;

public sealed class SpoolRepository : IDisposable
{
    private readonly ErrorDispatcher _errorDispatcher;
    private readonly SourceCache<SpoolData, string> _spools = new(sd => sd.Id);
    private readonly SourceCache<PendingOrder, string> _orders = new(po => po.Id);
    private readonly IBlobCache _blobCache = BlobCache.UserAccount;
    private readonly CompositeDisposable _subscriptions = new();

    private bool _isInitialized;
    private readonly object _lock = new();
    
    public SpoolRepository(ErrorDispatcher errorDispatcher) => _errorDispatcher = errorDispatcher;

    public void Init()
    {
        if(_isInitialized) return;

        lock (_lock)
        {
            if(_isInitialized) return;
            
            var spools = _blobCache.GetAllObjects<SpoolData>().ToEnumerable();
            var orders = _blobCache.GetAllObjects<PendingOrder>().ToEnumerable();

            _spools.Edit(
                e =>
                {
                    foreach (var spoolData in spools) e.AddOrUpdate(spoolData);
                });

            _orders.Edit(
                e =>
                {
                    foreach (var order in orders) e.AddOrUpdate(order);
                });

            StartVacoumTimer();
            CreateOrderSavePipeLine();
            CreateSpoolSavePipeLine();

            _isInitialized = true;
        }
    }

    private void StartVacoumTimer()
    {
        _subscriptions.Add
        (
            Observable.Timer(DateTimeOffset.Now, TimeSpan.FromHours(24))
                .SelectMany(_ => _blobCache.Vacuum().OnErrorResumeNext(Observable.Empty<Unit>()))
                .Subscribe()
        );
    }
    
    private void CreateSpoolSavePipeLine()
    {
        var data = _spools.Connect();
            if(_spools.Count != 0)
                data = data.SkipInitial();

            _subscriptions.Add
            (
                data
                    .OnItemRemoved(r => ReportError(_blobCache.Invalidate(r.Id)))
                    .OnItemAdded(a => ReportError(_blobCache.InsertObject(a.Id, a)))
                    .OnItemUpdated((u, _) => ReportError(_blobCache.InsertObject(u.Id, u)))
                    .Subscribe()
            );
    }

    private void CreateOrderSavePipeLine()
    {
        var data = _orders.Connect();
            if(_orders.Count != 0)
                data = data.SkipInitial();

            _subscriptions.Add
            (
                data
                    .OnItemRemoved(r => ReportError(_blobCache.Invalidate(r.Id)))
                    .OnItemAdded(a => ReportError(_blobCache.InsertObject(a.Id, a)))
                    .OnItemUpdated((u, _) => ReportError(_blobCache.InsertObject(u.Id, u)))
                    .Subscribe()
            );
    }
    
    private void ReportError(IObservable<Unit> toReport) => toReport.Subscribe(_ => { }, e => _errorDispatcher.Send(e));

    public IObservable<IChangeSet<SpoolData, string>> Spools => _spools.Connect().ObserveOn(Scheduler.Default);

    public IObservable<IChangeSet<PendingOrder, string>> Orders => _orders.Connect().ObserveOn(Scheduler.Default);

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
        
        string id = SpoolData.CreateId(name, category);
        return !_spools.Keys.Contains(id, StringComparer.Ordinal);
    }

    public void AddOrder(PendingOrder order)
        => _orders.AddOrUpdate(order);
    
    public void Dispose()
    {
        _subscriptions.Dispose();
        _spools.Dispose();
        _orders.Dispose();
    }

    public void Delete(SpoolData data) => _spools.Remove(data);

    public void Delete(PendingOrder order) => _orders.Remove(order);
}