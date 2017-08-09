using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Tauron.Application.Views;

namespace Tauron.Application.CelloManager.UI.Views.OptionsWindow.SubWindows
{
    /// <summary>
    ///     Interaktionslogik für LayoutOptionsView.xaml
    /// </summary>
    [ExportView(AppConststands.LayoutOptionsView)]
    public partial class LayoutOptionsView
    {
        public LayoutOptionsView()
        {
            InitializeComponent();
        }

        private void UIElement_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.All(char.IsDigit)) return;

            e.Handled = true;
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var grid = sender as DataGrid;

            var index = grid?.SelectedIndex;

            if (index != grid?.Items.Count - 1) return;

            if (grid == null) return;

            grid.CurrentCell = new DataGridCellInfo(grid.SelectedItem, grid.Columns[0]);
            grid.BeginEdit();
        }
    }
}