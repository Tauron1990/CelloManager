using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData.Alias;
using System.Threading.Tasks;
using Akavache;
using DynamicData;

namespace CelloManager.Avalonia.Core.Data;

public sealed class SpoolRepository : IDisposable
{
    private readonly SourceCache<SpoolData, string> _spools = new(sd => sd.Id);
    private readonly SourceCache<PendingOrder, string> _orders = new(po => po.Id);
    private readonly IBlobCache _blobCache = BlobCache.UserAccount;
    private readonly Subject<Exception> _errors = new();

    private CompositeDisposable _subscriptions = new();
    
    public async Task Init()
    {
        var spools = await _blobCache.GetAllObjects<SpoolData>();
        var orders = await _blobCache.GetAllObjects<PendingOrder>();
        
        _spools.Edit(e =>
        {
            foreach (var spoolData in spools) e.AddOrUpdate(spoolData);
        });
        
        _orders.Edit(e =>
        {
            foreach (var order in orders) e.AddOrUpdate(order);
        });

        StartVacoumTimer();
        CreateOrderSavePipeLine();
        CreateSpoolSavePipeLine();
    }

    private void StartVacoumTimer()
    {
        Observable.Timer(DateTimeOffset.Now, TimeSpan.FromHours(24))
            .SelectMany(_ => _blobCache.Vacuum().OnErrorResumeNext(Observable.Empty<Unit>()))
            .Subscribe().DisposeWith(_subscriptions);
    }
    
    private void CreateSpoolSavePipeLine()
    {
        var data = _spools.Connect().SkipInitial();

        data
            .OnItemRemoved(r => ReportError(_blobCache.Invalidate(r.Id)))
            .OnItemAdded(a => ReportError(_blobCache.InsertObject(a.Id, a)))
            .OnItemUpdated((u, _) => ReportError(_blobCache.InsertObject(u.Id, u)))
            .Subscribe().DisposeWith(_subscriptions);
    }

    private void CreateOrderSavePipeLine()
    {
        var data = _orders.Connect().SkipInitial();

        data
            .OnItemRemoved(r => ReportError(_blobCache.Invalidate(r.Id)))
            .OnItemAdded(a => ReportError(_blobCache.InsertObject(a.Id, a)))
            .OnItemUpdated((u, _) => ReportError(_blobCache.InsertObject(u.Id, u)))
            .Subscribe().DisposeWith(_subscriptions);
    }
    
    private void ReportError(IObservable<Unit> toReport) => toReport.Subscribe(_ => { }, e => _errors.OnNext(e));

    public IObservable<Exception> Errors => _errors.AsObservable();

    public IObservable<IChangeSet<SpoolData, string>> Spools => _spools.Connect();

    public IObservable<IChangeSet<PendingOrder, string>> Orders => _orders.Connect();

    public void UpdateSpool(SpoolData data) => _spools.AddOrUpdate(data);

    public void Dispose()
    {
        _errors.Dispose();
        _subscriptions.Dispose();
        _spools.Dispose();
        _orders.Dispose();
    }
}