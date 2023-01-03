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
using Material.Colors;
using Material.Styles.Themes;
using Material.Styles.Themes.Base;
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
    
    public PendingOrderViewModel(PendingOrder order, Func<PendingOrderViewModel, PendingOrderPrintView, ValueTask> print)
    {
        _print = print;
        Order = order;
        TimeOfOrder = order.Time.ToString("f", CultureInfo.CurrentUICulture);
        PrintCommand = ReactiveCommand.Create(
            () =>
            {
                _canPrint.OnNext(false);

                _preview = new Window
                {
                    Background = Brushes.White,
                    Foreground = Brushes.Black,
                };

                var control = new PendingOrderPrintView();
                control.Init(order);
                _preview.Content = control;

                _preview.Show();
                
                Task.Run(() => DoPrint(control));
            },
            _canPrint.ObserveOn(RxApp.MainThreadScheduler));
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
            await MessageBox.ShowAsync(App.MainWindow, $"Fehler: {ex.Message}", "Fehler", MessageBoxButton.Ok, MessageBoxImage.Error);
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
        => PrintCommand.Dispose();

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