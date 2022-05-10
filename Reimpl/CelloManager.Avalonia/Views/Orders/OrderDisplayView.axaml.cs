using System;
using System.Collections.Generic;
using Avalonia.ReactiveUI;
using CelloManager.Avalonia.ViewModels.Orders;
using ReactiveUI;

namespace CelloManager.Avalonia.Views.Orders;

public partial class OrderDisplayView : ReactiveUserControl<OrderDisplayViewModel>
{
    public OrderDisplayView()
    {
        InitializeComponent();
        this.WhenActivated(Init);
    }

    private IEnumerable<IDisposable> Init()
    {
        yield break;
    }
}