using System.Windows;
using System.Windows.Controls;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews
{
    /// <summary>
    ///     Interaktionslogik für SpoolViewWorkspaceView.xaml
    /// </summary>
    //[ExportView(AppConststands.SingleUISpoolViwName)]
    public partial class SingleUISpoolView
    {
        public SingleUISpoolView()
        {
            InitializeComponent();
        }

        private void DataObject_OnPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var text = (string)e.DataObject.GetData(typeof(string));
                if (!IsTextAllowed(text))
                    e.CancelCommand();
            }
            else
            {
                e.CancelCommand();
            }
        }

        private bool IsTextAllowed(string text)
        {
            int number;
            if (int.TryParse(text, out number))
                return number > 0;
            return false;
        }

        private void NumberBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            string text = NumberBox.Text;

            if(IsTextAllowed(text)) return;

            NumberBox.Text = "1";
        }
    }
}