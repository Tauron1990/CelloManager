using System.Globalization;
using Avalonia.Controls;
using CelloManager.Core.Data;

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
        SpoolList.ItemsSource = order.Spools;
    }
}