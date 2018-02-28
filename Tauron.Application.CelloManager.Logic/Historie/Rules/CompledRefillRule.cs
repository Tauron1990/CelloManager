using System;
using System.Linq;
using Tauron.Application.CelloManager.Data.Historie;
using Tauron.Application.CelloManager.Data.Manager;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;

namespace Tauron.Application.CelloManager.Logic.Historie.Rules
{
    [ExportRule(RuleNames.CompledRefillRule)]
    public class CompledRefillRule : IBusinessRuleBase<CommittedRefill>
    {
        public override void ActionImpl(CommittedRefill input)
        {
            using (var db = RepositoryFactory.Enter())
            {
                var spoolRepo = RepositoryFactory.GetRepository<ISpoolRepository>();
                var comRepo = RepositoryFactory.GetRepository<ICommittedRefillRepository>();

                var commitedRefill = comRepo.GetCommittedRefill(input.Id);
                commitedRefill.IsCompleted = true;
                commitedRefill.CompledTime = DateTime.Now;
                

                foreach (var entity in commitedRefill.CommitedSpools.Select(e => new {Ent = spoolRepo.Find(e.SpoolId), Count = e.OrderedCount, Spool = e}))
                {
                    var refillSpool = input.CommitedSpools.First(c => entity.Spool.SpoolId == c.SpoolId);
                    if (refillSpool.OrderedCount != entity.Spool.OrderedCount)
                        entity.Spool.OrderedCount = refillSpool.OrderedCount;

                    entity.Ent.Amount += entity.Count;
                }

                db.SaveChanges();
            }
        }
    }
}