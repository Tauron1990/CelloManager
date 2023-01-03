using System;
using System.Reactive;
using CelloManager.Core.Printing;
using ReactiveUI;

namespace CelloManager.ViewModels.Orders;

public sealed class SelectDocumentTypeViewMiodel : ViewModelBase
{
    public ReactiveCommand<DocumentType?, Unit> SelectCommand { get; }
    
    public SelectDocumentTypeViewMiodel(Action<DocumentType?> close)
    {
        SelectCommand = ReactiveCommand.Create<DocumentType?, Unit>(
            dt =>
            {
                close(dt);
                return Unit.Default;
            });
    }
}