using System;
using System.Linq;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.CelloManager.Data.Historie;
using Tauron.Application.CelloManager.Data.Manager;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;
using Tauron.Application.Ioc;

namespace Tauron.Application.CelloManager.Logic.Historie.Rules
{
    [ExportRule(RuleNames.PlaceOrderRule)]
    public sealed class PlaceOrderRule : OBuissinesRuleBase<CommittedRefill>
    {
        [Inject]
        public IManagerEnviroment ManagerEnviroment { get; set; }

        public override CommittedRefill ActionImpl()
        {
            var threshold = ManagerEnviroment.Settings.Threshold;

            using (var db = RepositoryFactory.Enter())
            {
                var spoolRepo = RepositoryFactory.GetRepository<ISpoolRepository>();
                var comRepo = RepositoryFactory.GetRepository<ICommittedRefillRepository>();
                var orders = comRepo.GetCommittedRefills(false).ToArray();

                CommittedRefillEntity ent = new CommittedRefillEntity {SentTime = DateTime.Now};

                foreach (var celloSpoolEntity in spoolRepo.QueryAsNoTracking()
                    .Select(s => new
                    {
                        Spool = s,
                        OrderedSpools = orders.SelectMany(c => c.CommitedSpools)
                            .Where(cs => cs.SpoolId == s.Id)
                    })
                    .Where(at => at.Spool.Neededamount > at.Spool.Amount + at.OrderedSpools.Sum(cs => cs.OrderedCount) + threshold))
                {
                    ent.CommitedSpools.Add(celloSpoolEntity.Spool.ConvertCommit());
                }

                if (ent.CommitedSpools.Count == 0) return null;

                comRepo.Add(ent);
                db.SaveChanges();

                return ent.CreateCommittedRefill();
            }
        }
    }
}