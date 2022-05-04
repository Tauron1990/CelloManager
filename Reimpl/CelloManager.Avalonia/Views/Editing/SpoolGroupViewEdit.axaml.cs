using System;
using System.Collections.Generic;
using Avalonia.ReactiveUI;
using CelloManager.Avalonia.ViewModels.Editing;
using ReactiveUI;

namespace CelloManager.Avalonia.Views.Editing;

public partial class EditSpoolGroupView : ReactiveUserControl<EditSpoolGroupViewModel>
{
    public EditSpoolGroupView()
    {
        InitializeComponent();

        this.WhenActivated(Init);
    }

    private IEnumerable<IDisposable> Init()
    {
        if(ViewModel is null) yield break;

        yield return this.OneWayBind(ViewModel, m => m.Spools, v => v.SpoolDataGrid.Items);
        yield return this.Bind(ViewModel, m => m.Selected, v => v.SpoolDataGrid.SelectedItem);
    }
}