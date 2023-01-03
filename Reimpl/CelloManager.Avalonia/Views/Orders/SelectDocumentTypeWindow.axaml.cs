using System;
using System.Collections.Generic;
using Avalonia.ReactiveUI;
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
        
    }
}