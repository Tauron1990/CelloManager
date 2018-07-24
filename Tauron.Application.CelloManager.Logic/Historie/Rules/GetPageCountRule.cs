using System.Linq;
using Tauron.Application.CelloManager.Data.Historie;
using Tauron.Application.CelloManager.Logic.Historie.DTO;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;

namespace Tauron.Application.CelloManager.Logic.Historie.Rules
{
    [ExportRule(RuleNames.GetPageCount)]
    public class GetPageCountRule : OBuissinesRuleBase<GetPageCountResult>
    {
        public override GetPageCountResult ActionImpl()
        {
            using (RepositoryFactory.Enter())
            {
                var repo = RepositoryFactory.GetRepository<ICommittedRefillRepository>();

                int reposCount = repo.QueryAsNoTracking().Count();
                int count;

                if (reposCount != 0)
                    count = reposCount / 20 + 1;
                else
                    count = 0;

                return new GetPageCountResult(count);
            }
        }
    }
}