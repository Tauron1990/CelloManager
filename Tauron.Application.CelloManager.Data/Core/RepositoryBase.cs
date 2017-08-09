using Tauron.Application.Ioc;

namespace Tauron.Application.CelloManager.Data.Core
{
    public abstract class RepositoryBase : IRepositoryBase
    {
        [Inject]
        public IDatabaseManager Manager { get; set; }

        protected DatabaseManager CoreManager => (DatabaseManager) Manager;
    }
}