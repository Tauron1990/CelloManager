using System.Windows;
using System.Windows.Input;

namespace Tauron.Application.CelloManager.Setup
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _model;

        public MainWindow()
        {
            InitializeComponent();
            _model = (MainWindowViewModel)DataContext;
            _model.SetWindow(this, WizardControl);
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            _model.OnCancel();
        }

        private void OnFinish(object sender, RoutedEventArgs e)
        {
            _model.OnFinish();
        }

        private void OnSelectedPageChanged(object sender, RoutedEventArgs e)
        {
            _model.OnSelectedPageChanged();
        }

        private void MainWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            _model.OnBrowseClick();
        }
    }
}
