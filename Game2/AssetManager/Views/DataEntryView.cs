using System;
using System.Collections.Generic;
using AssetManager.ViewModels;
using Avalonia.ReactiveUI;
using JetBrains.Annotations;
using ReactiveUI;

namespace AssetManager.Views;

[UsedImplicitly]
public partial class DataEntryView : ReactiveUserControl<DataEntryViewModel>
{
    public DataEntryView()
    {
        InitializeComponent();
        this.WhenActivated(InitView);
    }

    private IEnumerable<IDisposable> InitView()
    {
        if(ViewModel is null) yield break;

        yield return this.BindCommand(ViewModel, m => m.AddItem, v => v.AddItemButton);
        yield return this.BindCommand(ViewModel, m => m.EditItem, v => v.EditItemButton);
        yield return this.BindCommand(ViewModel, m => m.RemoveItem, v => v.RemoveItemButton);

        yield return this.OneWayBind(ViewModel, m => m.Entrys, v => v.Entrys.Items);
        yield return this.Bind(ViewModel, m => m.CurrentEntry, v => v.Entrys.SelectedItem);
    }
}