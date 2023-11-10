using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using QuestPDF.Infrastructure;

namespace CelloManager.Core.Printing.Impl;

public interface IInternalDocument : IPrintDocument, IDisposable
{
    public static abstract IPrintDocument GenerateDocument(IDocument pages);

    ValueTask Execute(Dispatcher dispatcher, Action end);
}