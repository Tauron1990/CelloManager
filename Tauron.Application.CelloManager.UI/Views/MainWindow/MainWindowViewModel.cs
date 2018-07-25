using System.ComponentModel;
using Syncfusion.Windows.Tools.Controls;
using Tauron.Application.CelloManager.UI.Models;
using Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews;
using Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews.OrderView;
using Tauron.Application.Models;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow
{
    [ExportViewModel(AppConststands.MainWindowName)]
    public sealed class MainWindowViewModel : ViewModelBase
    {
        private bool _editorVisible;
        private bool _ordersVisible;

        [InjectModel(UIModule.SpoolModelName)]
        public SpoolModel SpoolModel { get; set; }

        [InjectModel(UIModule.OperationContextModelName)]
        public OperationContextModel OperationContextModel { get; set; }

        public UISyncObservableCollection<DockItem> Tabs => SpoolModel.Views;


        private DockItem _editor;
        private DockItem _orders;

        private DockState _lastEditor;
        private DockState _lastOrders;

        public bool BlockStade { get; set; }

        public bool EditorVisible
        {
            get => _editorVisible;
            set
            {
                _editorVisible = value;
                OnPropertyChanged();
                Switch(_editor, ref _lastEditor, value);
            }
        }

        public bool OrdersVisible
        {
            get => _ordersVisible;
            set
            {
                _ordersVisible = value;
                OnPropertyChanged();
                Switch(_orders, ref _lastOrders, value);
            }
        }

        public override void BuildCompled()
        {
            _editor = Factory.Object<SpoolDataEditingViewModel>().GetDockItem();
            _orders = Factory.Object<OrderViewModel>().GetDockItem();

            _lastEditor = _editor.State;
            _lastOrders = _orders.State;

            DependencyPropertyDescriptor desc = DependencyPropertyDescriptor.FromProperty(DockItem.StateProperty, typeof(DockItem));


            desc.AddValueChanged(_editor, (sender, args) =>
                                          {
                                              if (BlockStade) return;

                                              BlockStade = true;
                                              if (_editor.State == DockState.Hidden)
                                              {
                                                  EditorVisible = false;
                                                  BlockStade = false;
                                                  return;
                                              }

                                              EditorVisible = true;
                                              _lastEditor = _editor.State;
                                              BlockStade = false;
                                          });

            desc.AddValueChanged(_orders, (sender, args) =>
                                          {
                                              if (BlockStade) return;

                                              BlockStade = true;
                                              if (_orders.State == DockState.Hidden)
                                              {
                                                  OrdersVisible = false;
                                                  BlockStade = false;
                                                  return;
                                              }

                                              OrdersVisible = true;
                                              _lastOrders = _orders.State;
                                              BlockStade = false;
                                          });

            Tabs.Add(_editor);
            Tabs.Add(_orders);
        }

        [CommandTarget]
        public void Close() => CommonApplication.Current.MainWindow?.Close();

        [CommandTarget]
        public void Settings() => ViewManager.CreateWindow(AppConststands.OptionsWindow).ShowDialogAsync(CommonApplication.Current.MainWindow);

        private void Switch(DockItem item, ref DockState last, bool value)
        {
            if (BlockStade) return;

            BlockStade = true;
            if (value)
            {
                if (last == DockState.Hidden)
                    last = DockState.AutoHidden;
                item.State = last;
            }
            else
            {
                last       = item.State;
                item.State = DockState.Hidden;
            }

            BlockStade = false;
        }
    }
}