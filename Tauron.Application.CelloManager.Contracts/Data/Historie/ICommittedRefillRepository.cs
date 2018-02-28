using System.Collections.Generic;
using Tauron.Application.Common.BaseLayer.Data;

namespace Tauron.Application.CelloManager.Data.Historie
{
    public interface ICommittedRefillRepository : IRepository<CommittedRefillEntity, int>
    {
        CommittedRefillEntity GetCommittedRefill(int id);

        IEnumerable<CommittedRefillEntity> GetCommittedRefills(bool compled);
    }
}