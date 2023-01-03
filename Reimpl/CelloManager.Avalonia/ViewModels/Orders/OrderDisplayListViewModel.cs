using System;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using CelloManager.Core.Movere;
using CelloManager.Core.Data;
using CelloManager.Core.Printing;
using CelloManager.Views.Orders;
using DynamicData;
using ReactiveUI;

namespace CelloManager.ViewModels.Orders;

public sealed class OrderDisplayListViewModel : ViewModelBase, IDisposable
{
    private readonly PrintBuilder _builder;
    private readonly IDisposable _subscription;

    public ReadOnlyObservableCollection<PendingOrderViewModel> Orders { get; }

    public OrderDisplayListViewModel(IObservable<IChangeSet<PendingOrder, string>> orders, PrintBuilder builder)
    {
        _builder = builder;
        _subscription = orders
            .Transform(po => new PendingOrderViewModel(po, StartPrint))
            .DisposeMany()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out var list)
            .Subscribe();

        Orders = list;
    }

    private async ValueTask StartPrint(PendingOrderViewModel model, PendingOrderPrintView pendingOrderPrintView)
    {
        var window = new SelectDocumentTypeWindow();
        var type = await window.ShowDialog<DocumentType?>(App.MainWindow);
        
        if(type is null) return;
        
        using IPrintDocument doc = _builder.GenerateDocument(type.Value, pendingOrderPrintView);
        await _builder.ExecuteDocument(doc, model.Close);
    }

    public void Dispose() => _subscription.Dispose();
}