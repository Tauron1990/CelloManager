using System;
using System.Collections.Generic;
using System.Linq;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.CelloManager.Data.Historie;
using Tauron.Application.CelloManager.Data.Manager;
using Tauron.Application.CelloManager.Logic.Historie.DTO;
using Tauron.Application.CelloManager.Logic.Manager;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;
using Tauron.Application.Ioc;

namespace Tauron.Application.CelloManager.Logic.Historie.Rules
{
    [ExportRule(RuleNames.IsRefillNeededRule)]
    public sealed class IsRefillNeededRule : IOBusinessRuleBase<IsRefillNeededInput, IsRefillNeededResult>
    {
        [Inject]
        public IManagerEnviroment ManagerEnviroment { get; set; }

        public override IsRefillNeededResult ActionImpl(IsRefillNeededInput input)
        {
            IEnumerable<CelloSpool> spools = input.Spools;
            IEnumerable<CommittedRefill> refills = input.Refills;
            IDisposable dbacess = null;

            try
            {
                if (spools == null || refills == null)
                {
                    dbacess = RepositoryFactory.Enter();
                    spools  = RepositoryFactory.GetRepository<ISpoolRepository>().QueryAsNoTracking().Select(e => e.CreateCelloSpool());
                    refills = RepositoryFactory.GetRepository<ICommittedRefillRepository>().GetCommittedRefills(false).Select(e => e.CreateCommittedRefill());
                }


                return new IsRefillNeededResult(IsRefillNeeded(spools, refills));
            }
            finally
            {
                dbacess?.Dispose();
            }
        }

        private bool IsRefillNeeded(IEnumerable<CelloSpool> spools, IEnumerable<CommittedRefill> refills)
        {
            var threshold = ManagerEnviroment.Settings.Threshold;

            return spools.Select(s => new
                                      {
                                          Spool = s, 
                                          OrderedSpools = refills.SelectMany(c => c.CommitedSpools)
                                                                 .Where(cs => cs.SpoolId == s.Id)
                                      })
                         .Any(at => at.Spool.Neededamount >= at.Spool.Amount + at.OrderedSpools.Sum(cs => cs.OrderedCount) + threshold);
        }
    }
}