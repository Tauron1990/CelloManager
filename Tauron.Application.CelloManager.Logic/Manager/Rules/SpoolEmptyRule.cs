using Tauron.Application.CelloManager.Data.Manager;
using Tauron.Application.CelloManager.Logic.Manager.DTO;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;

namespace Tauron.Application.CelloManager.Logic.Manager.Rules
{
    [ExportRule(RuleNames.SpoolEmptyRule)]
    public sealed class SpoolEmptyRule : IOBusinessRuleBase<RemoveAmountData, RemoveAmountResult>
    {
        public override RemoveAmountResult ActionImpl(RemoveAmountData input)
        {
            using (var db = RepositoryFactory.Enter())
            {
                var repo = RepositoryFactory.GetRepository<ISpoolRepository>();

                var entity = repo.Find(input.CelloSpool.Id);
                if( entity == null || entity.Amount <= input.Amount) return new RemoveAmountResult(false);

                entity.Amount -= input.Amount;

                db.SaveChanges();
            }

            return new RemoveAmountResult(true);
        }
    }
}