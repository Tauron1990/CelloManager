using System.Collections.Generic;
using Tauron.Application.CelloManager.Data.Manager;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;

namespace Tauron.Application.CelloManager.Logic.Manager.Rules
{
    [ExportRule(RuleNames.UpdateSpoolsRules)]
    public sealed class UpdateSpoolsRules : IBusinessRuleBase<IEnumerable<CelloSpool>>
    {
        public override void ActionImpl(IEnumerable<CelloSpool> input)
        {
            using (var db = RepositoryFactory.Enter())
            {
                var repo = RepositoryFactory.GetRepository<ISpoolRepository>();

                foreach (var celloSpool in input)
                {
                    var ent = repo.Find(celloSpool.Id);
                    if(ent == null) continue;

                    ent.Amount = celloSpool.Amount;
                    ent.Name = celloSpool.Name;
                    ent.Neededamount = celloSpool.Neededamount;
                    ent.Type = celloSpool.Type;
                }

                db.SaveChanges();
            }
        }
    }
}