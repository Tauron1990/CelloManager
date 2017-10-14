using System.Linq;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.Ioc;

namespace Tauron.Application.CelloManager.Data.Manager
{
    [Export(typeof(ISpoolRepository))]
    public sealed class SpoolRepository : ISpoolRepository
    {
        private readonly CoreDatabase _database;

        [Inject]
        public SpoolRepository(CoreDatabase database)
        {
            _database = database;
        }

        public CelloSpoolEntry Add()
        {
            var temp = new CelloSpoolEntry();

            _database.CelloSpools.Add(temp);
            return temp;
        }

        public void Remove(int id)
        {
            _database.CelloSpools.Remove(GetEntry(id));
        }

        public IQueryable<CelloSpoolEntry> GetSpools()
        {
            return _database.CelloSpools;
        }
        

        public CelloSpoolEntry GetEntry(int id)
        {
            var temp = _database.CelloSpools.Local.FirstOrDefault(e => e.Id == id);
            return temp ?? _database.CelloSpools.Single(c => c.Id == id);
        }
    }
}