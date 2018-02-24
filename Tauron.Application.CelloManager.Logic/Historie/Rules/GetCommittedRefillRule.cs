using System.Collections.Generic;
using System.Linq;
using Tauron.Application.CelloManager.Data.Historie;
using Tauron.Application.CelloManager.Logic.Historie.DTO;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;

namespace Tauron.Application.CelloManager.Logic.Historie.Rules
{
    [ExportRule(RuleNames.GetCommittedRefillRule)]
    public sealed class GetCommittedRefillRule : IOBusinessRuleBase<GetCommittedRefillFlag, IEnumerable<CommittedRefill>>
    {
        public override IEnumerable<CommittedRefill> ActionImpl(GetCommittedRefillFlag flag)
        {
            using (RepositoryFactory.Enter())
            {
                foreach (var refillEntity in RepositoryFactory.GetRepository<ICommittedRefillRepository>()
                                                      .GetCommittedRefills(true).Where(e => e.IsCompleted == flag.GetCompleted))
                {
                    yield return refillEntity.CreateCommittedRefill();
                }
            }
        }
    }
}