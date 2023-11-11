using System;
using System.Collections.Generic;
using Avalonia.ReactiveUI;
using CelloManager.ViewModels.SpoolDisplay;
using ReactiveUI;

namespace CelloManager.Views.SpoolDisplay;

public partial class SpoolGroupView : ReactiveUserControl<SpoolGroupViewModel>
{
    public SpoolGroupView()
    {
        InitializeComponent();

        this.WhenActivated(Init);
    }

    private IEnumerable<IDisposable> Init()
    {
        yield return this.OneWayBind(ViewModel, m => m.Spools, v => v.SpoolList.ItemsSource);
    }
}