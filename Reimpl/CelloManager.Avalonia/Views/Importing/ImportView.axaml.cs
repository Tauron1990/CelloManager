using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Avalonia.ReactiveUI;
using CelloManager.Core.Comp;
using CelloManager.ViewModels.Importing;
using ReactiveUI;

namespace CelloManager.Views.Importing;

public sealed partial class ImportView : ReactiveUserControl<ImportViewModel>
{
    public ImportView()
    {
        InitializeComponent();

        this.WhenActivated(Init);
    }

    private IEnumerable<IDisposable> Init()
    {
        if(ViewModel is null) yield break;
        
        yield return this.OneWayBind(ViewModel, m => m.IsRunning, v => v.ProgressBarControl.IsVisible);
        yield return this.OneWayBind(ViewModel, m => m.Error, v => v.ErrorText.Text);
        
        yield return this.BindCommand(ViewModel, m => m.Import, v => v.ImportDefault, Observable.Return(CoreDatabase.DefaultPath()));
        yield return this.BindCommand(ViewModel, m => m.Import, v => v.ImportFrom, Observable.Return((string)null!));
    }
}