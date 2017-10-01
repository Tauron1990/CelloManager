using System;
using System.Collections.Generic;
using System.Linq;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.CelloManager.Data.Historie;
using Tauron.Application.Ioc;

namespace TestHelpers.Mocks
{
    [Export(typeof(ICommittedRefillRepository))]
    public class CommittedSpoolRepositoryMock : ICommittedRefillRepository
    {
        private readonly CoreDatabase _database;
        private static List<CommittedRefill> _committedRefills;

        [Inject]
        public CommittedSpoolRepositoryMock(CoreDatabase database)
            : this(4)

        {
            _database = database;
        }

        public CommittedSpoolRepositoryMock(int toGenerate = -1)
        {
            DateTime start = DateTime.Now;
            if (_committedRefills != null) return;

            _committedRefills = new List<CommittedRefill>(toGenerate == -1 ? 0 : toGenerate);

            if(toGenerate == -1) return;

            DataShuffler shuffler = new DataShuffler();

            for (int i = 0; i < toGenerate; i++)
            {
                start = GetNext(start);
                var refill = new CommittedRefill { SentTime = start };

                for (int j = 0; j < shuffler.GetNumber(10); j++)
                {
                    refill.CommitedSpools.Add(new CommittedSpool { Name = shuffler.GetName(), Type = shuffler.GetType(), OrderedCount = shuffler.GetNumber() });
                }

                _committedRefills.Add(refill);
            }
        }

        private DateTime GetNext(DateTime prevorius)
        {
            return prevorius - new TimeSpan(1, 0, 0, 0);
        }
        
        public IQueryable<CommittedRefill> GetCommittedRefills()
        {
            return _committedRefills.AsQueryable();
        }

        public void Add(CommittedRefill data)
        {
            _committedRefills.Add(data);
        }

        public void Delete(CommittedRefill refill)
        {
            _committedRefills.Remove(refill);
        }
    }
}