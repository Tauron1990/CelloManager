using System.Windows.Controls;
using Tauron.Application.Views;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews
{
    /// <summary>
    /// Interaktionslogik für UpdaterContainerView.xaml
    /// </summary>
    [ExportView(AppConststands.UpdateView)]
    public partial class UpdaterContainerView : UserControl
    {
        public UpdaterContainerView()
        {
            InitializeComponent();
        }
    }
}
