using System;
using System.Globalization;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Avalonia.Threading;
using CelloManager.Core.Data;
using CelloManager.Core.Printing;
using CelloManager.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ThingLing.Controls;

namespace CelloManager.ViewModels.Orders;

public sealed class PendingOrderViewModel : ViewModelBase, IDisposable
{
    private readonly BehaviorSubject<bool> _canPrint = new(value: true);
    private readonly ErrorDispatcher _errors;
    private readonly ILogger<PendingOrderViewModel> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly Action<PendingOrder, bool> _commit;
    private readonly MainWindowProvider _provider;

    public PendingOrderViewModel(PendingOrder order, IServiceProvider serviceProvider, Action<PendingOrder, bool> commit, MainWindowProvider provider)
    {
        _serviceProvider = serviceProvider;
        _commit = commit;
        _provider = provider;
        _errors = serviceProvider.GetRequiredService<ErrorDispatcher>();
        _logger = serviceProvider.GetRequiredService<ILogger<PendingOrderViewModel>>();
        Order = order;
        TimeOfOrder = order.Time.ToString("f", CultureInfo.CurrentUICulture);

        CommitCommand = ReactiveCommand.CreateFromTask<bool>(ExecuteCommit);

        PrintCommand = ReactiveCommand.CreateFromTask(RunPrint,
            _canPrint.ObserveOn(RxApp.MainThreadScheduler));
        
    }

    private async Task ExecuteCommit(bool skipApplying)
    {
        if (skipApplying)
        {
            MessageBoxResult result = await MessageBox.ShowAsync(_provider.MainWindow, "Soll die Bestellung Gelöscht werden?", "Löschen", MessageBoxButton.YesNo, MessageBoxImage.Warning).ConfigureAwait(true);

            if (result == MessageBoxResult.Yes)
            {
                _commit(Order, true);
            }
        }
        else
        {
            _commit(Order, false);
        }
    }
    
    public PendingOrder Order { get; }

    public string TimeOfOrder { get; }

    public ReactiveCommand<Unit, Unit> PrintCommand { get; }

    public ReactiveCommand<bool, Unit> CommitCommand { get; }

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
            _logger.LogError(new EventId(5002, "OrderPrintCommandFailed"), e, "Order print command failed before the print workflow handled the error");
            _errors.Send(e);
        }
    }
}