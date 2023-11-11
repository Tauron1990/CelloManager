using System;
using System.Collections.Generic;
using Avalonia.ReactiveUI;
using CelloManager.ViewModels.Editing;
using ReactiveUI;

namespace CelloManager.Views.Editing;

public partial class EditTabView : ReactiveUserControl<EditTabViewModel>
{
    public EditTabView()
    {
        InitializeComponent();
        
        this.WhenActivated(Init);
    }

    private IEnumerable<IDisposable> Init()
    {
        if(ViewModel is null) yield break;
        
        yield return this.BindCommand(ViewModel, m => m.NewSpool, v => v.NewSpool);

        yield return this.Bind(ViewModel, m => m.CurrentSelected, v => v.SpoolTreeView.SelectedItem);
        yield return this.OneWayBind(ViewModel, m => m.SpoolGroups, v => v.SpoolTreeView.ItemsSource);

        yield return this.OneWayBind(ViewModel, m => m.CurrentEditorModel, v => v.EditorField.Content);
    }
}