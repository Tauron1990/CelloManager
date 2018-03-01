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

                int count = repo.Query().Count() / 20 + 1;

                return new GetPageCountResult(count);
            }
        }
    }
}