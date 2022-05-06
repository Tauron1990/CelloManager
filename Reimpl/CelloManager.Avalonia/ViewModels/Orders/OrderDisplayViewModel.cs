using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CelloManager.Avalonia.Core.Data;
using CelloManager.Avalonia.Core.Logic;
using DynamicData;
using DynamicData.Alias;
using ReactiveUI;

namespace CelloManager.Avalonia.ViewModels.Orders;

public sealed class OrderDisplayViewModel : ViewModelBase, IDisposable, ITabInfoProvider
{
    private readonly IDisposable _subscription;
    
    public ReadOnlyObservableCollection<PendingOrderViewModel> Orders { get; set; }
    
    public OrderDisplayViewModel(OrderManager manger)
    {
        _subscription = manger.Orders
            .Select(po => new PendingOrderViewModel(po, manger))
            .DisposeMany()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out var list)
            .Subscribe();

        Orders = list;
    }

    public void Dispose() => _subscription.Dispose();

    public string Title => "Bestellungen";
    public bool CanClose => true;

}

public sealed class PendingOrderViewModel : ViewModelBase, IDisposable
{
    public IEnumerable<OrderedSpool> Spools { get; }

    public string OrderTime { get; }

    public ReactiveCommand<Unit, Unit> Commit { get; }
    
    public PendingOrderViewModel(PendingOrder order, OrderManager manager)
    {
        OrderTime = order.Time.ToLocalTime().ToString("d");
        Spools = order.Spools;
        Commit = ReactiveCommand.Create(new Action(() => Task.Run(() => manager.CompledOrder(order))));
    }

    public void Dispose()
    {
        Commit.Dispose();
    }
}