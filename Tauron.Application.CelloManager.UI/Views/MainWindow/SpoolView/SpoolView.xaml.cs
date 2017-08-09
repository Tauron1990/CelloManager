using Tauron.Application.Views;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.SpoolView
{
    /// <summary>
    ///     Interaktionslogik für SpoolView.xaml
    /// </summary>
    [ExportView(AppConststands.SpoolView)]
    public partial class SpoolView
    {
        public SpoolView()
        {
            InitializeComponent();
            //((TabControl) this.Content).ItemsSource = ((SpoolViewModel) this.DataContext).SpoolTypes;
        }
    }
}