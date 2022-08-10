using System;
using AssetManager.ViewModels;
using Avalonia;
using Avalonia.ReactiveUI;

namespace AssetManager.Views;

public sealed partial class AssetsView : ReactiveUserControl<AssetsViewModel>
{
    public AssetsView()
    {
        InitializeComponent();
        this.GetObservable(ViewModelProperty).Subscribe(m => DataEntryView.ViewModel = m);
    }
}