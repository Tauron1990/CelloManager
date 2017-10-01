using System.Collections.Generic;
using System.Linq;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.CelloManager.Logic.Manager;
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

        public void UpdateEntry(CelloSpoolBase celloSpool)
        {
            var coreEntry = GetEntry(celloSpool.Id);

            celloSpool.Name = coreEntry.Name;
            celloSpool.Amount = coreEntry.Amount;
            celloSpool.Neededamount = coreEntry.Neededamount;
            celloSpool.Type = coreEntry.Type;
        }

        public CelloSpoolBase Add()
        {
            var temp = new CelloSpoolEntry();

            _database.CelloSpools.Add(temp);
            return new CelloSpool(temp, true);
        }

        public void Remove(CelloSpoolBase entry)
        {
            _database.CelloSpools.Remove(GetEntry(entry.Id));
        }

        public IEnumerable<CelloSpoolBase> GetSpools()
        {
            return _database.CelloSpools.Select(spool => new CelloSpool(spool, false)).ToArray();
        }

        private CelloSpoolEntry GetEntry(int id)
        {
            var temp = _database.CelloSpools.Local.FirstOrDefault(e => e.Id == id);
            return temp ?? _database.CelloSpools.Single(c => c.Id == id);
        }
    }
}