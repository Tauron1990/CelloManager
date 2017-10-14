using System;
using Tauron.Application.CelloManager.Data;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.CelloManager.Data.Historie;
using Tauron.Application.CelloManager.Data.Manager;

namespace TestHelpers.Mocks
{
    public class OperationMok : IOperation
    {
        public OperationMok(IOptionsRepository options, ISpoolRepository spools, ICommittedRefillRepository committedRefills)
        {
            Options = options;
            Spools = spools;
            CommittedRefills = committedRefills;
        }



        public IOptionsRepository Options { get; }
        public ISpoolRepository Spools { get; }
        public ICommittedRefillRepository CommittedRefills { get; }

        public void Commit()
        {
            
        }

        public void Fail()
        {

        }
    }

    public class OperationManagerMock : IOperationManager
    {
        private readonly IOperation _op;

        public OperationManagerMock(IOperation op)
        {
            _op = op;
        }

        public void Enter(Action<IOperation> operation)
        {
            operation(_op);
        }
    }
}