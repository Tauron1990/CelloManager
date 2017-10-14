using System;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.CelloManager.Data.Historie;
using Tauron.Application.CelloManager.Data.Manager;

namespace Tauron.Application.CelloManager.Data
{
    public interface IOperation
    {
        IOptionsRepository Options { get; }
        ISpoolRepository Spools { get; }
        ICommittedRefillRepository CommittedRefills { get; }

        void Commit();
        void Fail();
    }
}