using Tauron.Application.Views;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews
{
    /// <summary>
    ///     Interaktionslogik für SpoolViewWorkspaceView.xaml
    /// </summary>
    [ExportView(AppConststands.SpoolViewWorkspaceViewModel)]
    public partial class SpoolViewWorkspaceView
    {
        public SpoolViewWorkspaceView()
        {
            InitializeComponent();
        }
    }
}