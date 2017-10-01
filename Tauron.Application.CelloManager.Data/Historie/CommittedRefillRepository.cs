using System.Linq;
using Microsoft.EntityFrameworkCore;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.Ioc;

namespace Tauron.Application.CelloManager.Data.Historie
{
    [Export(typeof(ICommittedRefillRepository))]
    public sealed class CommittedRefillRepository : ICommittedRefillRepository
    {
        private readonly CoreDatabase _database;

        [Inject]
        public CommittedRefillRepository(CoreDatabase database)
        {
            _database = database;
        }

        public IQueryable<CommittedRefill> GetCommittedRefills()
        {
            return _database.CommittedRefills.Include(r => r.CommitedSpools);
        }

        public void Add(CommittedRefill data)
        {
            _database.CommittedRefills.Add(data);
        }

        public void Delete(CommittedRefill entity)
        {
            if (entity == null) return;

            _database.CommittedRefills.Remove(entity);
        }
    }
}