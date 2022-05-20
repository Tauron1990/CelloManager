using System;
using System.Collections.Generic;
using Avalonia.ReactiveUI;
using CelloManager.Avalonia.ViewModels.Orders;
using ReactiveUI;

namespace CelloManager.Avalonia.Views.Orders;

public partial class OrderDisplayListView : ReactiveUserControl<OrderDisplayListViewModel>
{
    public OrderDisplayListView()
    {
        InitializeComponent();

        this.WhenActivated(Init);
    }

    private IEnumerable<IDisposable> Init()
    {
        yield return this.OneWayBind(ViewModel, m => m.Orders, v => v.OrdersDisplay.Items);
    }
}