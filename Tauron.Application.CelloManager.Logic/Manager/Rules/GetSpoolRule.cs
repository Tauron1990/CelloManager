using System;
using System.Collections.Generic;
using System.Linq;
using Tauron.Application.CelloManager.Data.Manager;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;

namespace Tauron.Application.CelloManager.Logic.Manager.Rules
{
    [ExportRule(RuleNames.GetSpoolRule)]
    public sealed class GetSpoolRule : OBuissinesRuleBase<IEnumerable<CelloSpool>>
    {
        public override IEnumerable<CelloSpool> ActionImpl()
        {
            using (RepositoryFactory.Enter())
                return RepositoryFactory.GetRepository<ISpoolRepository>().QueryAsNoTracking().ToList().Select(Extensions.CreateCelloSpool);
        }
    }
}