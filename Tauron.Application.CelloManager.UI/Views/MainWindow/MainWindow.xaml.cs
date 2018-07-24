using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using Syncfusion.Windows.Tools.Controls;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.Ioc;
using Tauron.Application.Views;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow
{
    /// <summary>
    ///     Interaktionslogik für MainWindow.xaml
    /// </summary>
    [ExportWindow(AppConststands.MainWindowName)]
    public partial class MainWindow
    {
        [Inject]
        public IManagerEnviroment Enviroment { private get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {

        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            //DockingManager.DeleteDockState();
            DockingManager.DeleteInternalIsolatedStorage();

            using (var stringWriter = new StringWriter())
            {
                //DockingManager.ResetState();
                DockingManager.SaveDockState(stringWriter);
                Enviroment.Settings.DockingState = stringWriter.ToString();
                Enviroment.Save();
            }
        }

        private void DockingManager_OnLoaded(object sender, RoutedEventArgs e)
        {
            string text = Enviroment.Settings.DockingState;
            if (string.IsNullOrWhiteSpace(text)) return;
            DockingManager.LoadDockState(new StringReader(text));

            if(!(DataContext is MainWindowViewModel temp)) return;

            temp.BlockStade = true;
            temp.EditorVisible = FindDockItem(AppConststands.SpoolDataEditingView)?.State != DockState.Hidden;
            temp.OrdersVisible = FindDockItem(AppConststands.OrderView)?.State != DockState.Hidden;
            temp.BlockStade = false;
        }

        private DockItem FindDockItem(string name) => DockingManager.ItemsSource.FirstOrDefault(i => i.Name == name);
    }
}