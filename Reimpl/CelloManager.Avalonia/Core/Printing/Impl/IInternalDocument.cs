using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using CelloManager.Views.Orders;
using TempFileStream.Abstractions;

namespace CelloManager.Core.Printing.Impl;

public interface IInternalDocument : IPrintDocument, IDisposable
{
    public static abstract IPrintDocument GenerateDocument(ITempFile[] pages);

    ValueTask Execute(Dispatcher dispatcher, Action end);
}