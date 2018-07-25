using System.Collections.Generic;
using System.Linq;
using Tauron.Application.CelloManager.Data.Historie;
using Tauron.Application.CelloManager.Data.Manager;

namespace Tauron.Application.CelloManager.Logic.Core
{
    public static class ThresholdHelper
    {
        public static IQueryable<CelloSpoolEntity> FilterForOrder(this IQueryable<CelloSpoolEntity> spools, IEnumerable<CommittedRefillEntity> orders, int threshold)
        {
            //var spools2 = spools.ToArray();

            return spools.Select(s => new
                {
                    Spool = s,
                    OrderedSpools = orders.SelectMany(c => c.CommitedSpools)
                        .Where(cs => cs.SpoolId == s.Id)
                })
                .Where(at => at.Spool.Neededamount > at.Spool.Amount + at.OrderedSpools.Sum(cs => cs.OrderedCount) + GetThershold(at.Spool, threshold))
                .Select(t => t.Spool).AsQueryable();
        }

        private static int GetThershold(CelloSpoolEntity entity, int original)
        {
            for (int i = 0; i <= original; i++)
            {
                if(entity.Neededamount > i + 1) continue;

                if (i <= 1) return i;

                return i - 1;
            }

            return original;
        }
    }
}
