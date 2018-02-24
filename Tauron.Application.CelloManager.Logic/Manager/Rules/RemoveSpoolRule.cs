using System;
using Tauron.Application.CelloManager.Data.Manager;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;

namespace Tauron.Application.CelloManager.Logic.Manager.Rules
{
    [ExportRule(RuleNames.RemoveSpoolRule)]
    public sealed class RemoveSpoolRule : IBusinessRuleBase<CelloSpool>
    {
        public override void ActionImpl(CelloSpool input)
        {
            using (var db = RepositoryFactory.Enter())
            {
                var repo = RepositoryFactory.GetRepository<ISpoolRepository>();

                var ent = repo.Find(input.Id);
                if(ent == null) return;

                repo.Remove(ent);

                db.SaveChanges();
            }
        }
    }
}