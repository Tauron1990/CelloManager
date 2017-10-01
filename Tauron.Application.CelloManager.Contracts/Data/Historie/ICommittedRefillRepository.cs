using System.Linq;
using JetBrains.Annotations;

namespace Tauron.Application.CelloManager.Data.Historie
{
    public interface ICommittedRefillRepository
    {
        [ItemNotNull]
        [NotNull]
        IQueryable<CommittedRefill> GetCommittedRefills();

        void Add([NotNull] CommittedRefill data);
        void Delete([CanBeNull] CommittedRefill refill);
    }
}