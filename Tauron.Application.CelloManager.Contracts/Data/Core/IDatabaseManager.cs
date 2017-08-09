
using System;

namespace Tauron.Application.CelloManager.Data.Core
{
    public interface IDatabaseManager
    {
        bool SaveChanges { get; set; }

        IDisposable StartOperation();
    }
}