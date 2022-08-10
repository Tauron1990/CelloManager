using AssetManager.ViewModels;
using Avalonia;
using Avalonia.ReactiveUI;
using System;
using JetBrains.Annotations;

namespace AssetManager.Views;

[UsedImplicitly]
public sealed partial class ScriptsView : ReactiveUserControl<ScriptsViewModel>
{
    public ScriptsView()
    {
        InitializeComponent();
        this.GetObservable(ViewModelProperty).Subscribe(m => DataEntryView.ViewModel = m);
    }
}