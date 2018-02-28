using Tauron.Application.CelloManager.Data.Manager;
using Tauron.Application.CelloManager.Logic.Manager.DTO;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;

namespace Tauron.Application.CelloManager.Logic.Manager.Rules
{
    [ExportRule(RuleNames.AddAmountRule)]
    public sealed class AddAmountRule : IOBusinessRuleBase<AddAmountData, AddAmountResult>
    {
        public override AddAmountResult ActionImpl(AddAmountData input)
        {
            using (var db = RepositoryFactory.Enter())
            {
                var repo = RepositoryFactory.GetRepository<ISpoolRepository>();

                var ent = repo.Find(input.CelloSpool.Id);
                
                if(ent == null) return new AddAmountResult(false);

                ent.Amount += input.Amount;

                db.SaveChanges();

                return new AddAmountResult(true);
            }
        }
    }
}