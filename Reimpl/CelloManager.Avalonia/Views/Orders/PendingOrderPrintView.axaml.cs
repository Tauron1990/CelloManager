using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using CelloManager.Avalonia.Core.Data;
using CelloManager.Avalonia.ViewModels.Orders;
using ReactiveUI;

namespace CelloManager.Avalonia.Views.Orders;

public partial class PendingOrderPrintView : UserControl
{
    public PendingOrderPrintView(PendingOrder order)
    {
        InitializeComponent();

        Id.Text = $"Bestellung: {order.Id}";
        OrderTime.Text =  order.Time.ToString("f", CultureInfo.CurrentUICulture);
        SpoolList.Items = order.Spools;
    }
}