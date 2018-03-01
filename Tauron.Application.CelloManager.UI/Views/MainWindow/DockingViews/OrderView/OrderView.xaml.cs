using Syncfusion.UI.Xaml.Controls.DataPager;
using Syncfusion.UI.Xaml.TreeGrid;
using Tauron.Application.Views;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews.OrderView
{
    /// <summary>
    /// Interaktionslogik für OrderView.xaml
    /// </summary>
    [ExportView(AppConststands.OrderView)]
    public partial class OrderView
    {
        public OrderView()
        {
            InitializeComponent();
        }

        private void DaterPager_OnOnDemandLoading(object sender, OnDemandLoadingEventArgs e)
        {
            ((OrderViewModel)DataContext).OnDemandLoading((SfDataPager)sender, e);
        }
    }
}
