using System.Linq;
using Microsoft.EntityFrameworkCore;
using Tauron.Application.Common.BaseLayer.Data;

namespace Tauron.Application.CelloManager.Data.Historie
{
    public sealed class CommittedRefillRepository : Repository<CommittedRefillEntity, int>, ICommittedRefillRepository
    {
        public IQueryable<CommittedRefillEntity> GetCommittedRefills(bool asNoTracking)
        {
            return (asNoTracking ? QueryAsNoTracking() : Query()).Include(r => r.CommitedSpools);
        }

        public CommittedRefillRepository(IDatabase database) 
            : base(database)
        {
        }
    }
}