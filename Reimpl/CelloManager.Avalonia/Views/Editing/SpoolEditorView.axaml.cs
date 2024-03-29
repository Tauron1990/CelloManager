﻿using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using CelloManager.ViewModels.Editing;
using ReactiveUI;

namespace CelloManager.Views.Editing;

public partial class SpoolEditorView : ReactiveUserControl<ModifySpoolEditorViewModel>
{
    public SpoolEditorView()
    {
        InitializeComponent();
        void OnAthorGotFocus(object? sender, GotFocusEventArgs args) => CategoryPopup.IsOpen = false;

        NameTextBox.GotFocus += OnAthorGotFocus;
        SaveButton.GotFocus += OnAthorGotFocus;
        AmountTextBox.GotFocus += OnAthorGotFocus;
        NeedAmountTextBox.GotFocus += OnAthorGotFocus;

        this.WhenActivated(Init);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        CategoryPopup.IsOpen = false;
        base.OnDetachedFromVisualTree(e);
    }

    private IEnumerable<IDisposable> Init()
    {
        if(ViewModel is null) yield break;
        
        yield return this.BindCommand(ViewModel, m => m.Save, v => v.SaveButton);
        yield return this.BindCommand(ViewModel, m => m.Delete, v => v.DeleteButton);

        yield return this.Bind(ViewModel, m => m.Name, v => v.NameTextBox.Text);
        yield return this.Bind(ViewModel, m => m.Category, v => v.CategoryTextBox.Text);
        yield return this.Bind(ViewModel, m => m.Amount, v => v.AmountTextBox.Text);
        yield return this.Bind(ViewModel, m => m.NeedAmount, v => v.NeedAmountTextBox.Text);

        yield return this.OneWayBind(ViewModel, m => m.KnowenCategorys, v => v.PopupCategoryList.ItemsSource);
        yield return this.Bind(ViewModel, m => m.PopupSelection, v => v.PopupCategoryList.SelectedIndex, 
            i => i ?? 0,
            i => i);
        
        CategoryTextBox.AddHandler(TextInputEvent, CategoryTextBox_OnTextInput, RoutingStrategies.Tunnel);
        yield return Disposable.Create(this, el => el.CategoryTextBox.RemoveHandler(TextInputEvent, el.CategoryTextBox_OnTextInput));
    }

    private void CategoryTextBox_OnTextInput(object? sender, TextInputEventArgs e)
    {
        if (ViewModel?.KnowenCategorys.Count != 0) CategoryPopup.IsOpen = true;
    }
}