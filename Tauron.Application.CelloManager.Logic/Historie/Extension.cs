using System.Linq;
using Tauron.Application.CelloManager.Data.Historie;
using Tauron.Application.CelloManager.Data.Manager;

namespace Tauron.Application.CelloManager.Logic.Historie
{
    public static class Extensions
    {
        public static CommittedRefill CreateCommittedRefill(this CommittedRefillEntity refillEntity) =>
            new CommittedRefill(refillEntity.CommitedSpools
                                            .Select(e => new CommittedSpool(e.Name, e.OrderedCount, e.Type, e.SpoolId))
                                            .ToArray(), refillEntity.SentTime, refillEntity.CompledTime, refillEntity.Id);

        public static CommittedSpoolEntity ConvertCommit(this CelloSpoolEntity entity) =>
            new CommittedSpoolEntity(entity.Name, entity.Neededamount - entity.Amount, entity.Type, entity.Id);
    }
}