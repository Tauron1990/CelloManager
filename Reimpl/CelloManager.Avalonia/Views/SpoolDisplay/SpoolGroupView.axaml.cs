using System;
using System.Collections.Generic;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using CelloManager.Avalonia.ViewModels.SpoolDisplay;
using ReactiveUI;

namespace CelloManager.Avalonia.Views.SpoolDisplay;

public partial class SpoolGroupView : ReactiveUserControl<SpoolGroupViewModel>
{
    public SpoolGroupView()
    {
        InitializeComponent();

        this.WhenActivated(Init);
    }

    private IEnumerable<IDisposable> Init()
    {
        yield return this.OneWayBind(ViewModel, m => m.Spools, v => v.SpoolList.Items);
    }
}