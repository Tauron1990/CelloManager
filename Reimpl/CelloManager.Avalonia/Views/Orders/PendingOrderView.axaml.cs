using System;
using System.Collections.Generic;
using Avalonia.ReactiveUI;
using CelloManager.Avalonia.ViewModels.Orders;
using ReactiveUI;

namespace CelloManager.Avalonia.Views.Orders;

public partial class PendingOrderView : ReactiveUserControl<PendingOrderViewModel>
{
    public PendingOrderView()
    {
        InitializeComponent();

        this.WhenActivated(Init);
    }

    private IEnumerable<IDisposable> Init()
    {
        yield return this.OneWayBind(ViewModel, m => m.TimeOfOrder, v => v.OrderTime.Text);
#pragma warning disable CS8631
#pragma warning disable CS8634
        yield return this.BindCommand(ViewModel, m => m!.PrintCommand, v => v.PrintOrder.Command);
#pragma warning restore CS8634
#pragma warning restore CS8631
        yield return this.OneWayBind(ViewModel, m => m.Order.Spools, v => v.SpoolList.Items);
    }
}