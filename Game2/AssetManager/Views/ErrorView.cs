using System;
using System.Collections.Generic;
using AssetManager.ViewModels;
using Avalonia.ReactiveUI;
using JetBrains.Annotations;
using ReactiveUI;

namespace AssetManager.Views;

[UsedImplicitly]
public sealed partial class ErrorView : ReactiveUserControl<ErrorViewModel>
{
    public ErrorView()
    {
        InitializeComponent();
        this.WhenActivated(InitView);
    }

    private IEnumerable<IDisposable> InitView()
    {
        if(ViewModel is null) yield break;

        yield return this.BindCommand(ViewModel, m => m.CloseCommand, v => v.Exit);
        yield return this.OneWayBind(ViewModel, m => m.Message, v => v.ErrorContent.Text);
    }
}