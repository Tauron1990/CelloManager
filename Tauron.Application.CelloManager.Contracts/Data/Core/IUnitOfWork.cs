using System;
using Tauron.Application.CelloManager.Data.Historie;
using Tauron.Application.CelloManager.Data.Manager;

namespace Tauron.Application.CelloManager.Data.Core
{
    public interface IUnitOfWork : IDisposable
    {
        ISpoolRepository SpoolRepository { get; }

        IOptionsRepository OptionsRepository { get; }
        
        ICommittedRefillRepository CommittedRefillRepository { get; }

        void Commit();
    }
}