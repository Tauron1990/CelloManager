using System;
using System.Collections.Generic;
using Avalonia.ReactiveUI;
using CelloManager.ViewModels.Orders;
using ReactiveUI;

namespace CelloManager.Views.Orders;

public partial class PendingOrderView : ReactiveUserControl<PendingOrderViewModel>
{
    public PendingOrderView()
    {
        InitializeComponent();

        this.WhenActivated(Init);
    }

    private IEnumerable<IDisposable> Init()
    {
        if(ViewModel is null) throw new InvalidOperationException($"No ViewModel {nameof(PendingOrderView)}");
        
        yield return this.OneWayBind(ViewModel, m => m.TimeOfOrder, v => v.OrderTime.Text);
        yield return this.OneWayBind(ViewModel, m => m.Order.Spools, v => v.SpoolList.Items);
        
        yield return this.BindCommand(ViewModel, m => m.PrintCommand, v => v.PrintOrder);
        yield return this.BindCommand(ViewModel, m => m.CommitCommand, v => v.Commit);
    }
}