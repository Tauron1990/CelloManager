using System;
using System.Collections.Generic;
using AssetManager.ViewModels;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace AssetManager.Views
{
    public sealed partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            this.WhenActivated(InitView);
        }

        private IEnumerable<IDisposable> InitView()
        {
            if(ViewModel is null) yield break;

            yield return this.OneWayBind(ViewModel, m => m.CurrentPath, v => v.PathTextField.Text);
            yield return this.OneWayBind(ViewModel, m => m.Scripts, v => v.ScriptsTab.Content);
            yield return this.OneWayBind(ViewModel, m => m.Assets, v => v.AssetTab.Content);
        }
    }
}