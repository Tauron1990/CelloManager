using Tauron.Application.CelloManager.Data.Historie;
using Tauron.Application.CelloManager.Data.Manager;
using Tauron.Application.Ioc;

namespace Tauron.Application.CelloManager.Data.Core
{
    [Export(typeof(IUnitOfWork)), NotShared]
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ISpoolRepository _spoolRepository;
        private readonly IOptionsRepository _optionsRepository;
        private readonly ICommittedRefillRepository _committedRefillRepository;
        private readonly CoreDatabase _coreDatabase;


        public UnitOfWork()
        {
            _coreDatabase = new CoreDatabase();

            _spoolRepository = new SpoolRepository(_coreDatabase);
            _committedRefillRepository = new CommittedRefillRepository(_coreDatabase);
            _optionsRepository = new OptionsRepository(_coreDatabase);
        }

        public void Dispose()
        {
            _coreDatabase.Dispose();
        }

        public ISpoolRepository SpoolRepository => _spoolRepository;// ?? (_spoolRepository = Container.Resolve<ISpoolRepository>(new SimpleBuildPrameter(_coreDatabase)));

        public IOptionsRepository OptionsRepository => _optionsRepository;// ?? (_optionsRepository = Container.Resolve<IOptionsRepository>(new SimpleBuildPrameter(_coreDatabase)));

        public ICommittedRefillRepository CommittedRefillRepository => _committedRefillRepository;// ?? (_committedRefillRepository = Container.Resolve<ICommittedRefillRepository>(new SimpleBuildPrameter(_coreDatabase)));

        public void Commit()
        {
            _coreDatabase.SaveChanges();
        }
    }
}