using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using CelloManager.Core.Data;
using CelloManager.Core.Logic;
using CelloManager.Core.Printing;
using CelloManager.ViewModels.SpoolDisplay;
using DynamicData;
using ReactiveUI;

namespace CelloManager.ViewModels.Orders;

public sealed class OrderDisplayListViewModel : ViewModelBase, IDisposable
{
    private readonly OrderManager _manger;
    private readonly MainWindowViewModel _mainWindow;
    private readonly IDisposable _subscription;

    public ReadOnlyObservableCollection<PendingOrderViewModel> Orders { get; }

    public OrderDisplayListViewModel(IObservable<IChangeSet<PendingOrder, string>> orders, PrintBuilder builder, OrderManager manger, MainWindowViewModel mainWindow)
    {
        _manger = manger;
        _mainWindow = mainWindow;
        _subscription = orders
            .Transform(po => new PendingOrderViewModel(
                po, 
                builder.StartPrint,
                DoCompledOrder))
            .DisposeMany()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out var list)
            .Subscribe();

        Orders = list;
    }

    private void DoCompledOrder(PendingOrder order)
    {
        _manger.CompledOrder(order);
        _mainWindow.DisplayTab<SpoolDisplayViewModel>();
    }
    

    public void Dispose() => _subscription.Dispose();
}