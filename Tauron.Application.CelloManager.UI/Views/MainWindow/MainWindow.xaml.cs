using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews;
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
        public IManagerEnviroment Enviroment { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            //SkinStorage.SetVisualStyle(this, "Metro");

            //var helper = new VisualStudio2015SkinHelper();
            //List<List<string>> xamlThemes = new List<List<string>>
            //{
            //    helper.GetDictionaries("MSControls", string.Empty),
            //    helper.GetDictionaries("DockingManager", string.Empty),
            //    helper.GetDictionaries("DocumentContainer", string.Empty),
            //    helper.GetDictionaries("ChromelessWindow", string.Empty),
            //    helper.GetDictionaries("SfDataGrid", string.Empty),
            //    helper.GetDictionaries("PivotGridControl", string.Empty)
            //};

            //foreach (var theme in xamlThemes.SelectMany(list => list).Distinct())
            //{
            //    Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(theme, UriKind.RelativeOrAbsolute) });
            //}
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            //DockingManager.DeleteDockState();
            DockingManager.DeleteInternalIsolatedStorage();

            using (var stringWriter = new StringWriter())
            {
                //DockingManager.ResetState();
                DockingManager.SaveDockState(stringWriter);
                Enviroment.Settings.DockingState = string.Empty;//stringWriter.ToString();
                Enviroment.Save();
            }
        }

        private void DockingManager_OnLoaded(object sender, RoutedEventArgs e)
        {
            string text = Enviroment.Settings.DockingState;
            if (string.IsNullOrWhiteSpace(text)) return;
            DockingManager.LoadDockState(new StringReader(text));

            //foreach (var child in DockingManager.Children)
            //{
            //    var child2 = (ContentControl) child;
            //    if (!(child2.Content is SpoolDataEditingView)) continue;

            //    var temp = VisualTreeHelper.GetParent(child2);
            //    var temp2 = temp as DockPanel;
            //    if (temp2 == null)
            //    {
            //        if (temp == null) continue;
            //        temp = VisualTreeHelper.GetParent(temp);
            //        temp2 = temp as DockPanel;
            //        if (temp2 == null)
            //        {
            //            if (temp == null) continue;
            //            temp2 = VisualTreeHelper.GetParent(temp) as DockPanel;
            //        }
            //    }
            //    temp2?.SetBinding(MinWidthProperty, new Binding(nameof(MinWidth)) { Source = child2.Content }).UpdateTarget();
            //}
        }
    }
}