using System.Collections.Generic;
using System.Linq;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.CelloManager.Data.Manager;
using Tauron.Application.Ioc;

namespace TestHelpers.Mocks
{
    [Export(typeof(ISpoolRepository)), NotShared]
    public class SpoolRepositoryMock : ISpoolRepository
    {
        private readonly List<CelloSpoolEntry> _spoolBases;
        private int _id;

        [Inject]
        public SpoolRepositoryMock(CoreDatabase database)
            : this(4)
        {
            
        }

        public SpoolRepositoryMock(int toGenerate = -1)
        {
            _id = 0;

            _spoolBases = new List<CelloSpoolEntry>(toGenerate == -1 ? 0 : toGenerate);

            if(toGenerate == -1) return;
            DataShuffler shuffler = new DataShuffler();

            for (int i = 0; i < toGenerate; i++)
            {
                _id++;

                var spool = new CelloSpoolEntry
                {
                    Id =  _id,
                    Neededamount = shuffler.GetNumberMin(3),
                    Name = shuffler.GetName(),
                    Type = shuffler.GetType()
                };

                spool.Amount = shuffler.GetNumber(spool.Neededamount);

                _spoolBases.Add(spool);
            }
        }

        public CelloSpoolEntry Add()
        {
            _id++;
            var spool = new CelloSpoolEntry { Id = _id };
            _spoolBases.Add(spool);
            return spool;
        }

        public void Remove(int entry)
        {
            _spoolBases.Remove(_spoolBases.First(s => s.Id == entry));
        }

        public IQueryable<CelloSpoolEntry> GetSpools()
        {
            return _spoolBases.AsQueryable();
        }

        public CelloSpoolEntry GetEntry(int entry)
        {
            return _spoolBases.First(s => s.Id == entry);
        }
    }
}