using System;

namespace AutoReleaser
{
    /// <summary>
    ///     Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnInitialized(object sender, EventArgs e)
        {
            ConsoleBox.Document = ((MainWindowViewModel)DataContext).Document;
            ConsoleBox.TextChanged += (o, args) => ConsoleBox.ScrollToEnd();
        }
    }
}