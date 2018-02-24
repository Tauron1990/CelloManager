using Tauron.Application.CelloManager.Data.Manager;
using Tauron.Application.CelloManager.Logic.Manager.DTO;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;

namespace Tauron.Application.CelloManager.Logic.Manager.Rules
{
    [ExportRule(RuleNames.AddAmountRule)]
    public sealed class AddAmountRule : IBusinessRuleBase<AddAmountData>
    {
        public override void ActionImpl(AddAmountData input)
        {
            using (var db = RepositoryFactory.Enter())
            {
                var repo = RepositoryFactory.GetRepository<ISpoolRepository>();

                var ent = repo.Find(input.CelloSpool.Id);
                
                ent.Amount += input.Amount;

                db.SaveChanges();
            }
        }
    }
}