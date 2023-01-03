using System;

namespace CelloManager.Core.Printing;

public interface IPrintDocument : IDisposable
{
    DocumentType Type { get; }
}