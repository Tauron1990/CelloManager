using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.ReactiveUI;
using CelloManager.ViewModels;
using CelloManager.Views.Controls;
using ReactiveUI;

namespace CelloManager.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        private readonly AutoSelectTabControl _autoSelectTabControl = new();
        
        public MainWindow()
        {
            InitializeComponent();

            _autoSelectTabControl.Init(MainContentTabs);
            
            this.WhenActivated(Init);
            
            IEnumerable<IDisposable> Init()
            {
                if(ViewModel is null) yield break;

                
                yield return this.OneWayBind(ViewModel, m => m.Tabs, v => v.MainContentTabs.Items);
                yield return this.OneWayBind(ViewModel, m => m.ErrorSimple, v => v.ErrorDisplay.Content);
                yield return ErrorDisplay.Bind(
                    ToolTip.TipProperty,
                    ViewModel.WhenAny(m => m.ErrorFull, c => c.Value).Select(e => new BindingValue<object?>(e)));

                yield return this.BindCommand(ViewModel, m => m.Edit, v => v.EditSpools);
                yield return this.BindCommand(ViewModel, m => m.Import, v => v.ImportOld);
                yield return this.BindCommand(ViewModel, m => m.Order, v => v.StartOrder);
                yield return this.BindCommand(ViewModel, m => m.Orders, v => v.DisplayOrders);
            }
        }
    }
}