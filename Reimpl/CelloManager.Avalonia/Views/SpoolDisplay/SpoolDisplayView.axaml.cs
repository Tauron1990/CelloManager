using System;
using System.Collections.Generic;
using Avalonia.ReactiveUI;
using CelloManager.ViewModels.SpoolDisplay;
using ReactiveUI;

namespace CelloManager.Views.SpoolDisplay;

public partial class SpoolDisplayView : ReactiveUserControl<SpoolDisplayViewModel>
{
    public SpoolDisplayView()
    {
        InitializeComponent();

        this.WhenActivated(Init);
    }

    private IEnumerable<IDisposable> Init()
    {
        yield return this.OneWayBind(ViewModel, m => m.Groups, v => v.CategoryDisplay.ItemsSource);
    }
}