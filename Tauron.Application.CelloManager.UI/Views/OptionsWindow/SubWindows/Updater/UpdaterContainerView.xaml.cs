using System.Windows.Controls;
using Tauron.Application.Views;

namespace Tauron.Application.CelloManager.UI.Views.OptionsWindow.SubWindows.Updater
{
    /// <summary>
    /// Interaktionslogik für UpdaterContainerView.xaml
    /// </summary>
    [ExportView(AppConststands.UpdateOptionsView)]
    public partial class UpdaterContainerView : UserControl
    {
        public UpdaterContainerView()
        {
            InitializeComponent();
        }
    }
}
