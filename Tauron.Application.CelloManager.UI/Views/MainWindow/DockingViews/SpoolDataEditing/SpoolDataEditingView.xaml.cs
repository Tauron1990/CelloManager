using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Syncfusion.UI.Xaml.Grid;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.Ioc;
using Tauron.Application.Views;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews
{
    /// <summary>
    /// Interaktionslogik für SpoolDataEditingView.xaml
    /// </summary>
    [ExportView(AppConststands.SpoolDataEditingView)]
    public partial class SpoolDataEditingView : UserControl
    {
        [Inject]
        public IManagerEnviroment Enviroment { get; set; }

        public SpoolDataEditingView()
        {
            InitializeComponent();
        }

        private void Grid_OnLoaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.MainWindow.Closing += MainWindowOnClosing;

            string text = Enviroment.Settings.SpoolDataGridState;

            if(string.IsNullOrWhiteSpace(text)) return;

            using (MemoryStream stream = new MemoryStream(Encoding.Default.GetBytes(text)))
            {
                stream.Seek(0, SeekOrigin.Begin);
                DataGrid.Deserialize(stream);
            }
        }

        private void MainWindowOnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                DataGrid.Serialize(stream);
                Enviroment.Settings.SpoolDataGridState = Encoding.Default.GetString(stream.ToArray());
            }
            Enviroment.Save();
        }
    }
}
