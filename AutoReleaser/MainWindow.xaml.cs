using System;
using System.Threading.Tasks;

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
            var temp = (MainWindowViewModel) DataContext;

            ConsoleBox.Document = temp.Document;
            ConsoleBox.TextChanged += (o, args) => ConsoleBox.ScrollToEnd();

            Task.Run(new Action(temp.SetProjects));
            temp.Install();
        }
    }
}