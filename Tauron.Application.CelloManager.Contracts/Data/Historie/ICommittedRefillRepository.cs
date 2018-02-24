using System.Linq;
using JetBrains.Annotations;
using Tauron.Application.Common.BaseLayer.Data;

namespace Tauron.Application.CelloManager.Data.Historie
{
    public interface ICommittedRefillRepository : IRepository<CommittedRefillEntity, int>
    {
        [ItemNotNull]
        [NotNull]
        IQueryable<CommittedRefillEntity> GetCommittedRefills(bool noTracking);
    }
}