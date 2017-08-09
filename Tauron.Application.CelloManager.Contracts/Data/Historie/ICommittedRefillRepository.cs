using System.Linq;
using JetBrains.Annotations;
using Tauron.Application.CelloManager.Data.Core;

namespace Tauron.Application.CelloManager.Data.Historie
{
    public interface ICommittedRefillRepository : IRepositoryBase
    {
        [ItemNotNull]
        [NotNull]
        IQueryable<CommittedRefill> GetCommittedRefills();

        void Add([NotNull] CommittedRefill data);
        void Delete([CanBeNull] CommittedRefill refill);
    }
}