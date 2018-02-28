using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Tauron.Application.Common.BaseLayer.Data;

namespace Tauron.Application.CelloManager.Data.Historie
{
    public sealed class CommittedRefillRepository : Repository<CommittedRefillEntity, int>, ICommittedRefillRepository
    {
        public CommittedRefillRepository(IDatabase database) 
            : base(database)
        {
        }

        public CommittedRefillEntity GetCommittedRefill(int id) => Query().Include(e => e.CommitedSpools).First(e => e.Id == id);
        public IEnumerable<CommittedRefillEntity> GetCommittedRefills(bool compled) => QueryAsNoTracking().Include(cr => cr.CommitedSpools).Where(e => e.IsCompleted == compled);
    }
}