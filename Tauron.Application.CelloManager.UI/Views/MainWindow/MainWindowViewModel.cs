using Syncfusion.Windows.Tools.Controls;
using Tauron.Application.CelloManager.Logic.Manager;
using Tauron.Application.CelloManager.UI.Models;
using Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews;
using Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews.OrderView;
using Tauron.Application.Ioc;
using Tauron.Application.Models;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow
{
    [ExportViewModel(AppConststands.MainWindowName)]
    public sealed class MainWindowViewModel : ViewModelBase
    {
        [InjectModel(UIModule.SpoolModelName)]
        public SpoolModel SpoolModel { get; set; }

        public UISyncObservableCollection<DockItem> Tabs => SpoolModel.Views;

        //[CommandTarget]
        //public void Options()
        //{
        //    var window = ViewManager.CreateWindow(AppConststands.OptionsWindow);
        //    window.ShowDialogAsync(CommonApplication.Current.MainWindow);
        //}

        public override void BuildCompled()
        {
            Tabs.AddRange(new[]
            {
                Factory.Object<SpoolDataEditingViewModel>().GetDockItem(),
                Factory.Object<OrderViewModel>().GetDockItem()
            });
        }
    }
}