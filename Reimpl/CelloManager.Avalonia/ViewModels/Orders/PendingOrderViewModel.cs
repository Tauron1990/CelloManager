using System;
using System.Globalization;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Avalonia.Threading;
using CelloManager.Core.Data;
using CelloManager.Core.Printing;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace CelloManager.ViewModels.Orders;

public sealed class PendingOrderViewModel : ViewModelBase, IDisposable
{
    private readonly BehaviorSubject<bool> _canPrint = new(true);
    private readonly ErrorDispatcher _errors;
    private readonly IServiceProvider _serviceProvider;

    public PendingOrderViewModel(PendingOrder order, IServiceProvider serviceProvider, Action<PendingOrder> commit)
    {
        _serviceProvider = serviceProvider;
        _errors = serviceProvider.GetRequiredService<ErrorDispatcher>();
        Order = order;
        TimeOfOrder = order.Time.ToString("f", CultureInfo.CurrentUICulture);

        CommitCommand = ReactiveCommand.Create(() => commit(order));


        PrintCommand = ReactiveCommand.CreateFromTask(RunPrint,
            _canPrint.ObserveOn(RxApp.MainThreadScheduler));
    }

    public PendingOrder Order { get; }

    public string TimeOfOrder { get; }

    public ReactiveCommand<Unit, Unit> PrintCommand { get; }

    public ReactiveCommand<Unit, Unit> CommitCommand { get; }

    public void Dispose()
    {
        PrintCommand.Dispose();
        CommitCommand.Dispose();
    }

    private async Task RunPrint()
    {
        try
        {
            await PrintBuilder.PrintPendingOrder(Order, Dispatcher.UIThread, _serviceProvider, null).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _errors.Send(e);
        }
    }
}