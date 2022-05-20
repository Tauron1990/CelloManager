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
    
    public ReadOnlyObservableCollection<PendingOrder> Orders { get; }
    
    public OrderDisplayListViewModel(IObservable<IChangeSet<PendingOrder, string>> orders)
    {
        _subscription = orders
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out var list)
            .Subscribe();

        Orders = list;
    }

    public void Dispose() => _subscription.Dispose();
}