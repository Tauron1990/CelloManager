using System.Linq;
using Microsoft.EntityFrameworkCore;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.Ioc;

namespace Tauron.Application.CelloManager.Data.Historie
{
    [Export(typeof(ICommittedRefillRepository))]
    public sealed class CommittedRefillRepository : RepositoryBase, ICommittedRefillRepository
    {
        public IQueryable<CommittedRefill> GetCommittedRefills()
        {
            using(CoreManager.StartOperation())
                return CoreManager.Database.CommittedRefills.Include(r => r.CommitedSpools);
        }

        public void Add(CommittedRefill data)
        {
            using (CoreManager.StartOperation())
                CoreManager.Database.CommittedRefills.Add(data);
        }

        public void Delete(CommittedRefill entity)
        {
            if (entity == null) return;

            using (CoreManager.StartOperation())
                CoreManager.Database.CommittedRefills.Remove(entity);
        }
    }
}