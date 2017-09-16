using System;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
using Tauron.Application.Views;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews
{
    /// <summary>
    /// Interaktionslogik für SpoolDataEditingView.xaml
    /// </summary>
    [ExportView(AppConststands.SpoolDataEditingView)]
    public partial class SpoolDataEditingView : UserControl
    {
        public SpoolDataEditingView()
        {
            InitializeComponent();
        }

        private void Grid_OnLoaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.MainWindow.Closing += MainWindowOnClosing;

            using (var storeFile = IsolatedStorageFile.GetUserStoreForAssembly().OpenFile("Grid.xml", FileMode.OpenOrCreate))
            {
                if(storeFile.Length == 0) return;
                DataGrid.Deserialize(storeFile);
            }
        }

        private void MainWindowOnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            using (var storeFile = IsolatedStorageFile.GetUserStoreForAssembly().OpenFile("Grid.xml", FileMode.OpenOrCreate))
                DataGrid.Serialize(storeFile);
        }
    }
}
