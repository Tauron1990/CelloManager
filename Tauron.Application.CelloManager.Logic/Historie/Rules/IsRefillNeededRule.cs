using System;
using System.Collections.Generic;
using System.Linq;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.CelloManager.Data.Historie;
using Tauron.Application.CelloManager.Data.Manager;
using Tauron.Application.CelloManager.Logic.Core;
using Tauron.Application.CelloManager.Logic.Historie.DTO;
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
            IQueryable<CelloSpoolEntity> spools = input.Spools?.Select(cs => cs.CreateEntity()).AsQueryable();
            IEnumerable<CommittedRefillEntity> refills = input.Refills?.Select(cr => cr.CreateEntity()).ToArray();
            IDisposable dbacess = null;

            try
            {
                if (spools == null || refills == null)
                {
                    dbacess = RepositoryFactory.Enter();
                    spools  = RepositoryFactory.GetRepository<ISpoolRepository>().QueryAsNoTracking();
                    refills = RepositoryFactory.GetRepository<ICommittedRefillRepository>().GetCommittedRefills(false).ToArray();
                }


                return new IsRefillNeededResult(spools.FilterForOrder(refills, ManagerEnviroment.Settings.Threshold).Any());
            }
            finally
            {
                dbacess?.Dispose();
            }
        }
    }
}