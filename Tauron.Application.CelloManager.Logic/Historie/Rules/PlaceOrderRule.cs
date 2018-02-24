using System;
using System.Linq;
using Tauron.Application.CelloManager.Data.Historie;
using Tauron.Application.CelloManager.Data.Manager;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;

namespace Tauron.Application.CelloManager.Logic.Historie.Rules
{
    [ExportRule(RuleNames.PlaceOrderRule)]
    public sealed class PlaceOrderRule : OBuissinesRuleBase<CommittedRefill>
    {
        public override CommittedRefill ActionImpl()
        {
            using (var db = RepositoryFactory.Enter())
            {
                var spoolRepo = RepositoryFactory.GetRepository<ISpoolRepository>();
                var comRepo = RepositoryFactory.GetRepository<ICommittedRefillRepository>();

                CommittedRefillEntity ent = new CommittedRefillEntity {SentTime = DateTime.Now};

                foreach (var celloSpoolEntity in spoolRepo.QueryAsNoTracking().Where(e => e.Amount < e.Neededamount))
                    ent.CommitedSpools.Add(celloSpoolEntity.ConvertCommit());

                if (ent.CommitedSpools.Count == 0) return null;

                comRepo.Add(ent);
                db.SaveChanges();

                return ent.CreateCommittedRefill();
            }
        }
    }
}