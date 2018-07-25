using System.Collections.Generic;
using System.Linq;
using Tauron.Application.CelloManager.Data.Manager;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;

namespace Tauron.Application.CelloManager.Logic.Manager.Rules
{
    [ExportRule(RuleNames.AddSpoolRule)]
    public sealed class AddSpoolRule : IOBusinessRuleBase<IEnumerable<CelloSpool>, IEnumerable<CelloSpool>>
    {
        public override IEnumerable<CelloSpool> ActionImpl(IEnumerable<CelloSpool> input)
        {
            List<CelloSpool> spools = new List<CelloSpool>();

            using (var db = RepositoryFactory.Enter())
            {
                List<CelloSpoolEntity> newSpools = new List<CelloSpoolEntity>();

                foreach (var inputSpool in input)
                {
                    var ent = inputSpool.CreateEntity();

                    var repo = RepositoryFactory.GetRepository<ISpoolRepository>();

                    repo.Add(ent);
                    newSpools.Add(ent);
                }

                db.SaveChanges();
                spools.AddRange(newSpools.Select(ns => ns.CreateCelloSpool()));
            }

            return spools;
        }
    }
}