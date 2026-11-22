using System;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace CelloManager.Core.Printing.Impl;

public interface IInternalDocument : IPrintDocument
{
    ValueTask Execute(Dispatcher dispatcher, Action end);
}