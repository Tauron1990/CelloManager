using System.Linq;
using Tauron.Application.CelloManager.Data.Manager;
using Tauron.Application.CelloManager.Logic.Historie.DTO;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;

namespace Tauron.Application.CelloManager.Logic.Historie.Rules
{
    [ExportRule(RuleNames.IsRefillNeededRule)]
    public sealed class IsRefillNeededRule : OBuissinesRuleBase<IsRefillNeededResult>
    {
        public override IsRefillNeededResult ActionImpl()
        {
            using (RepositoryFactory.Enter())
                return new IsRefillNeededResult(RepositoryFactory.GetRepository<ISpoolRepository>().QueryAsNoTracking().Any(e => e.Neededamount > e.Amount));
        }
    }
}