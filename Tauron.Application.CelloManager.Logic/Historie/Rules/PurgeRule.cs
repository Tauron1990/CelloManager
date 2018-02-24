using System.Linq;
using Tauron.Application.CelloManager.Data.Historie;
using Tauron.Application.CelloManager.Logic.Historie.DTO;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;

namespace Tauron.Application.CelloManager.Logic.Historie.Rules
{
    [ExportRule(RuleNames.PurgeRule)]
    public sealed class PurgeRule : IBusinessRuleBase<PurgeSettings>
    {
        public override void ActionImpl(PurgeSettings input)
        {
            if(input.Maximum == -1) return;

            using (var db = RepositoryFactory.Enter())
            {
                var repository = RepositoryFactory.GetRepository<ICommittedRefillRepository>();
                var maxamount = input.Maximum;
                var count     = repository.GetCommittedRefills(false).Count();

                if (count < maxamount) return;

                foreach (var purge in repository.GetCommittedRefills(false).OrderBy(r => r.SentTime).Take(count - maxamount))
                    repository.Remove(purge);

                db.SaveChanges();
            }
        }
    }
}