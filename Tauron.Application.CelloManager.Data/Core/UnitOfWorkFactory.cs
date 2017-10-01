using Tauron.Application.Ioc;

namespace Tauron.Application.CelloManager.Data.Core
{
    [Export(typeof(IUnitOfWorkFactory))]
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        [Inject]
        public IContainer Container { get; set; }

        public IUnitOfWork CreateUnitOfWork()
        {
            return Container.Resolve<IUnitOfWork>();
        }
    }
}