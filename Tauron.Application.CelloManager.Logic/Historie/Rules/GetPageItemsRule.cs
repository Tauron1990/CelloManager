using System.Linq;
using Tauron.Application.CelloManager.Data.Historie;
using Tauron.Application.CelloManager.Logic.Historie.DTO;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;

namespace Tauron.Application.CelloManager.Logic.Historie.Rules
{
    [ExportRule(RuleNames.GetPageItems)]
    public class GetPageItemsRule : IOBusinessRuleBase<GetPageItemsData, GetPageItemsResult>
    {
        public override GetPageItemsResult ActionImpl(GetPageItemsData input)
        {
            int toSkip = input.Page; //(input.Page - 1) * 20;

            using (RepositoryFactory.Enter())
            {
                var repo = RepositoryFactory.GetRepository<ICommittedRefillRepository>();

                return new GetPageItemsResult(repo.GetCommittedRefills(true)
                                                  .OrderBy(e => e.CompledTime)
                                                  .Skip(toSkip)
                                                  .Take(20)
                                                  .Select(e => e.CreateCommittedRefill())
                                                  .ToArray());
            }
        }
    }
}