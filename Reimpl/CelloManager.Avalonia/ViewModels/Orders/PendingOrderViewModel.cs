using System;
using System.Globalization;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using CelloManager.Core.Data;
using CelloManager.Views.Orders;
using ReactiveUI;
using ThingLing.Controls;

namespace CelloManager.ViewModels.Orders;

public sealed class PendingOrderViewModel : ViewModelBase, IDisposable
{
    private readonly Func<PendingOrderViewModel, PendingOrderPrintView, ValueTask> _print;
    private readonly BehaviorSubject<bool> _canPrint = new(true);

    private Window? _preview;
    
    public PendingOrder Order { get; }

    public string TimeOfOrder { get; }

    public ReactiveCommand<Unit,Unit> PrintCommand { get; }
    
    public ReactiveCommand<Unit, Unit> CommitCommand { get; }
    
    public PendingOrderViewModel(PendingOrder order, Func<PendingOrderViewModel, PendingOrderPrintView, ValueTask> print, Action<PendingOrder> commit)
    {
        _print = print;
        Order = order;
        TimeOfOrder = order.Time.ToString("f", CultureInfo.CurrentUICulture);

        CommitCommand = ReactiveCommand.Create(() => commit(order));
        

        PrintCommand = ReactiveCommand.Create(
            RunPrint,
            _canPrint.ObserveOn(RxApp.MainThreadScheduler));
    }

    public void RunPrint()
    {
        _canPrint.OnNext(false);

        _preview = new Window
        {
            SizeToContent = SizeToContent.WidthAndHeight,
            Background = Brushes.White,
            Foreground = Brushes.Black,
            Title = "Blid Vorschau",
            CanResize = false,
            ShowInTaskbar = false,
        };

        var control = new PendingOrderPrintView();
        control.Init(Order);
        _preview.Content = control;

        _preview.Show();

        Task.Run(() => DoPrint(control));
    }
    
    private async  ValueTask DoPrint(PendingOrderPrintView view)
    {
        try
        {
            await Task.Delay(500);
            await _print(this, view).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await Dispatcher.UIThread
                .InvokeAsync(() => MessageBox.ShowAsync(App.MainWindow, $"Fehler: {ex.Message}", "Fehler", MessageBoxButton.Ok, MessageBoxImage.Error));
            Dispatcher.UIThread.Post(
                () =>
                {
                    _preview?.Close();
                    _preview = null;
                });
        }
        finally
        {
            _canPrint.OnNext(true);
        }
    }
    
    public void Dispose()
    {
        PrintCommand.Dispose();
        CommitCommand.Dispose();
    }

    public void Close()
    {
        Dispatcher.UIThread.Post(
            () =>
            {
                _preview?.Close();
                _preview = null;
            });
    }
}