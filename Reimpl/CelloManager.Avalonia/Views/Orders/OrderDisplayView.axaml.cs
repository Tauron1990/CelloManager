using System;
using System.Collections.Generic;
using Avalonia.ReactiveUI;
using CelloManager.ViewModels.Orders;
using ReactiveUI;

namespace CelloManager.Views.Orders;

public partial class OrderDisplayView : ReactiveUserControl<OrderDisplayViewModel>
{
    public OrderDisplayView()
    {
        InitializeComponent();
        this.WhenActivated(Init);
    }

    private IEnumerable<IDisposable> Init()
    {
        yield return this.OneWayBind(ViewModel, m => m.CurrentContent, v => v.OrdersDiplayer.Content);
    }
}