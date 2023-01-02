using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using CelloManager.Avalonia.Core.Data;
using DynamicData;
using ReactiveUI;

namespace CelloManager.Avalonia.ViewModels.Orders;

public sealed class OrderDisplayListViewModel : ViewModelBase, IDisposable
{
    private readonly IDisposable _subscription;

    public ReadOnlyObservableCollection<PendingOrderViewModel> Orders { get; }

    public OrderDisplayListViewModel(IObservable<IChangeSet<PendingOrder, string>> orders)
    {
        _subscription = orders
            .Transform(po => new PendingOrderViewModel(po, StartPrint))
            .DisposeMany()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out var list)
            .Subscribe();

        Orders = list;
    }

    private void StartPrint(PendingOrder obj)
    {
        throw new NotImplementedException();
    }

    public void Dispose() => _subscription.Dispose();
}