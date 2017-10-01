using System;
using System.Collections.Generic;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.CelloManager.Data.Manager;
using Tauron.Application.CelloManager.Logic.Manager;
using Tauron.Application.Ioc;

namespace TestHelpers.Mocks
{
    [Export(typeof(ISpoolRepository)), NotShared]
    public class SpoolRepositoryMock : ISpoolRepository
    {
        private class SimpleSpool : CelloSpoolBase
        {
            public SimpleSpool(int id)
            {
                Id = id;
            }

            public override string UniquieId => BuildUinqueId(Name, Type);
            public override string Name { get; set; }
            public override string Type { get; set; }
            public override int Amount { get; set; }
            public override int Neededamount { get; set; }
            public override int Id { get; }

            public override void UpdateSpool(IUnitOfWork work)
            {
                
            }

            public override string ToString()
            {
                return Id.ToString();
            }
        }

        private readonly List<CelloSpoolBase> _spoolBases;
        private int _id;

        [Inject]
        public SpoolRepositoryMock(CoreDatabase database)
            : this(4)
        {
            
        }

        public SpoolRepositoryMock(int toGenerate = -1)
        {
            _id = 0;

            _spoolBases = new List<CelloSpoolBase>(toGenerate == -1 ? 0 : toGenerate);

            if(toGenerate == -1) return;
            DataShuffler shuffler = new DataShuffler();

            for (int i = 0; i < toGenerate; i++)
            {
                _id++;

                var spool = new SimpleSpool(_id)
                {
                    Neededamount = shuffler.GetNumberMin(3),
                    Name = shuffler.GetName(),
                    Type = shuffler.GetType()
                };

                spool.Amount = shuffler.GetNumber(spool.Neededamount);

                _spoolBases.Add(spool);
            }
        }

        public void UpdateEntry(CelloSpoolBase celloSpool)
        {
            
        }

        public CelloSpoolBase Add()
        {
            _id++;
            var spool = new SimpleSpool(_id);
            _spoolBases.Add(spool);
            return spool;
        }

        public void Remove(CelloSpoolBase entry)
        {
            _spoolBases.Remove(entry);
        }

        public IEnumerable<CelloSpoolBase> GetSpools()
        {
            return _spoolBases;
        }
    }
}