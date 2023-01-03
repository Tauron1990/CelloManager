using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using CelloManager.Core.Movere.ViewModels;

namespace CelloManager.Core.Movere.Views
{
    public partial class PrintDialog : ReactiveWindow<PrintDialogViewModel>
    {
        public PrintDialog()
        {
            InitializeComponent();
        }

        //private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}
