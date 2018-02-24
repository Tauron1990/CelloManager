using Tauron.Application.CelloManager.Data.Manager;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;

namespace Tauron.Application.CelloManager.Logic.Manager.Rules
{
    [ExportRule(RuleNames.AddSpoolRule)]
    public sealed class AddSpoolRule : IOBusinessRuleBase<CelloSpool, CelloSpool>
    {
        public override CelloSpool ActionImpl(CelloSpool input)
        {
            var ent = input.CreateEntity();
            using (var db = RepositoryFactory.Enter())
            {
                var repo = RepositoryFactory.GetRepository<ISpoolRepository>();

                repo.Add(ent);
                db.SaveChanges();
            }

            return ent.CreateCelloSpool();
        }
    }
}