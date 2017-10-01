using Tauron.Application.CelloManager.Data;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.CelloManager.Data.Historie;
using Tauron.Application.CelloManager.Data.Manager;

namespace TestHelpers.Mocks
{
    public class UnitOfWorkFactoryMock : IUnitOfWorkFactory
    {
        private class UnitOfWorkMock : IUnitOfWork
        {
            public UnitOfWorkMock(ISpoolRepository spoolRepository, IOptionsRepository optionsRepository, ICommittedRefillRepository committedRefillRepository)
            {
                SpoolRepository = spoolRepository;
                OptionsRepository = optionsRepository;
                CommittedRefillRepository = committedRefillRepository;
            }

            public void Dispose()
            {
                
            }

            public ISpoolRepository SpoolRepository { get; }
            public IOptionsRepository OptionsRepository { get; }
            public ICommittedRefillRepository CommittedRefillRepository { get; }
            public void Commit()
            {
            }
        }
        
        public ISpoolRepository SpoolRepository { get; set; }
        public ICommittedRefillRepository CommittedRefillRepository { get; set; }
        public IOptionsRepository OptionsRepository { get; set; }

        public UnitOfWorkFactoryMock()
        {
            
        }

        public IUnitOfWork CreateUnitOfWork()
        {
            return new UnitOfWorkMock(SpoolRepository, OptionsRepository, CommittedRefillRepository);
        }
    }
}