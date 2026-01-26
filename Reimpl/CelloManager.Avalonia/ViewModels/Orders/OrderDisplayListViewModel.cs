using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using CelloManager.Core.Data;
using CelloManager.Core.Logic;
using CelloManager.ViewModels.SpoolDisplay;
using DynamicData;
using ReactiveUI;

namespace CelloManager.ViewModels.Orders;

public sealed class OrderDisplayListViewModel : ViewModelBase, IDisposable
{
    private readonly MainWindowViewModel _mainWindow;
    private readonly OrderManager _manger;
    private readonly IDisposable _subscription;

    public OrderDisplayListViewModel(
        IObservable<IChangeSet<PendingOrder, string>> orders,
        IServiceProvider serviceProvider,
        OrderManager manger,
        MainWindowViewModel mainWindow)
    {
        _manger = manger;
        _mainWindow = mainWindow;
        _subscription = orders
                       .Transform(po => new PendingOrderViewModel(po, serviceProvider, DoCompledOrder))
                       .DisposeMany()
                       .ObserveOn(RxApp.MainThreadScheduler)
                       .Bind(out ReadOnlyObservableCollection<PendingOrderViewModel> list)
                       .Subscribe();

        Orders = list;
    }

    public ReadOnlyObservableCollection<PendingOrderViewModel> Orders { get; }


    public void Dispose() => _subscription.Dispose();

    private void DoCompledOrder(PendingOrder order)
    {
        _manger.CompledOrder(order);
        _mainWindow.DisplayTab<SpoolDisplayViewModel>();
    }
}