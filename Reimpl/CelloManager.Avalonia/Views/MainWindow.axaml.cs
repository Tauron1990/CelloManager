using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.ReactiveUI;
using CelloManager.Avalonia.ViewModels;
using ReactiveUI;

namespace CelloManager.Avalonia.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();

            this.WhenActivated(Init);
            
            IEnumerable<IDisposable> Init()
            {
                if(ViewModel is null) yield break;
                
                yield return this.Bind(ViewModel, m => m.CurrentTab, w => w.MainContentTabs.SelectedIndex);
                yield return this.OneWayBind(ViewModel, m => m.Tabs, v => v.MainContentTabs.Items);
                yield return this.OneWayBind(ViewModel, m => m.ErrorSimple, v => v.ErrorDisplay.Header);
                yield return ErrorDisplay.Bind(
                    ToolTip.TipProperty,
                    ViewModel.WhenAny(m => m.ErrorFull, c => c.Value).Select(e => new BindingValue<object?>(e)));
            }
        }
    }
}