using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Avalonia.ReactiveUI;
using CelloManager.Core.Printing;
using CelloManager.ViewModels.Orders;
using ReactiveUI;

namespace CelloManager.Views.Orders;

public partial class SelectDocumentTypeWindow : ReactiveWindow<SelectDocumentTypeViewMiodel>
{
    public SelectDocumentTypeWindow()
    {
        InitializeComponent();

        ViewModel = new SelectDocumentTypeViewMiodel(d => Close(d));

        this.WhenActivated(Init);
    }

    private IEnumerable<IDisposable> Init()
    {
        if(ViewModel is null) yield break;

        yield return this.BindCommand(ViewModel, m => m.SelectCommand, v => v.NewPdF, WithType(DocumentType.PDF));
        yield return this.BindCommand(ViewModel, m => m.SelectCommand, v => v.NewImage, WithType(DocumentType.Image));
        yield return this.BindCommand(ViewModel, m => m.SelectCommand, v => v.NewPrint, WithType(DocumentType.Print));
        yield return this.BindCommand(ViewModel, m => m.SelectCommand, v => v.Cancel, WithType(null));
    }
    
    private static IObservable<DocumentType?> WithType(DocumentType? type)
        => Observable.Return(type);
}