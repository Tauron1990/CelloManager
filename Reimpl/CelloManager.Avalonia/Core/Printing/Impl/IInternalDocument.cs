using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using CelloManager.Views.Orders;

namespace CelloManager.Core.Printing.Impl;

public interface IInternalDocument : IPrintDocument, IDisposable
{
    public static abstract IPrintDocument GenerateDocument(PendingOrderPrintView view);

    ValueTask Execute(Dispatcher dispatcher, Action end);
}