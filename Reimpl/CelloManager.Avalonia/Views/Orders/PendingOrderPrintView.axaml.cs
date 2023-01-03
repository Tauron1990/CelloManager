using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using CelloManager.ViewModels.Orders;
using CelloManager.Core.Data;
using ReactiveUI;

namespace CelloManager.Views.Orders;

public partial class PendingOrderPrintView : UserControl
{
    public PendingOrderPrintView()
    {
        InitializeComponent();
    }
    
    public void Init(PendingOrder order)
    {
        Id.Text = $"Bestellung: {order.Id}";
        OrderTime.Text =  order.Time.ToString("f", CultureInfo.CurrentUICulture);
        SpoolList.Items = order.Spools;
    }
}