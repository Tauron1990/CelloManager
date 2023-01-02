using System;
using System.Globalization;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using CelloManager.Avalonia.Core.Data;
using ReactiveUI;

namespace CelloManager.Avalonia.ViewModels.Orders;

public sealed class PendingOrderViewModel : ViewModelBase, IDisposable
{
    private readonly Action<PendingOrder> _print;
    private readonly BehaviorSubject<bool> _canPrint = new(true);

    public PendingOrder Order { get; }

    public string TimeOfOrder { get; }

    public ReactiveCommand<Unit,Unit> PrintCommand { get; }
    
    public PendingOrderViewModel(PendingOrder order, Action<PendingOrder> print)
    {
        _print = print;
        Order = order;
        TimeOfOrder = order.Time.ToString("f", CultureInfo.CurrentUICulture);
        PrintCommand = ReactiveCommand.Create(
            () =>
            {
                _canPrint.OnNext(false);
                Task.Run(DoPrint);
            },
            _canPrint.ObserveOn(RxApp.MainThreadScheduler));
    }

    private void DoPrint()
    {
        try
        {
            _print(Order);
        }
        finally
        {
            _canPrint.OnNext(true);
        }
    }
    
    public void Dispose() 
        => PrintCommand.Dispose();
}