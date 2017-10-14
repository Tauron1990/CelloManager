using System;
using Tauron.Application.CelloManager.Data.Historie;
using Tauron.Application.CelloManager.Data.Manager;

namespace Tauron.Application.CelloManager.Data.Core
{
    public sealed class Operation : IOperation
    {
        private readonly Action _commit;
        private readonly Action _fail;

        public Operation(IOptionsRepository options, ISpoolRepository spools, ICommittedRefillRepository committedRefills, Action commit, Action fail)
        {
            _commit = commit;
            _fail = fail;
            Options = options;
            Spools = spools;
            CommittedRefills = committedRefills;
        }

        public IOptionsRepository Options { get; }
        public ISpoolRepository Spools { get; }
        public ICommittedRefillRepository CommittedRefills { get; }

        public void Commit()
        {
            _commit();
        }

        public void Fail()
        {
            _fail();
        }
    }
}