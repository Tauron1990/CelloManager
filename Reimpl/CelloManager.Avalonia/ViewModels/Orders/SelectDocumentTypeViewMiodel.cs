using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using CelloManager.Core.Printing;
using ReactiveUI;

namespace CelloManager.ViewModels.Orders;

public sealed class SelectDocumentTypeViewMiodel : ViewModelBase
{
    public ReactiveCommand<DocumentType?, Unit> SelectCommand { get; }

    //public ReactiveCommand<DocumentType?, Unit> WindowsSelectCommand { get; }

    public SelectDocumentTypeViewMiodel(Action<DocumentType?> close)
    {
        SelectCommand = ReactiveCommand.Create<DocumentType?, Unit>(Execute);

        // WindowsSelectCommand = ReactiveCommand.Create<DocumentType?, Unit>(
        //     Execute, Observable.Return(RuntimeInformation.IsOSPlatform(OSPlatform.Windows)));
        return;

        Unit Execute(DocumentType? dt)
        {
            close(dt);
            return Unit.Default;
        }
    }
}