using System.Collections.Generic;
using Tauron.Application.CelloManager.Logic.Manager.DTO;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.BusinessLayer;
using Tauron.Application.Ioc;

namespace Tauron.Application.CelloManager.Logic.Manager
{
    [Export(typeof(ISpoolManager))]
    public class SpoolManager : ISpoolManager
    {
        [InjectRuleFactory]
        public RuleFactory RuleFactory { private get; set; }

        public IEnumerable<CelloSpool> CelloSpools => RuleFactory.CreateOBussinesRule<IEnumerable<CelloSpool>>(RuleNames.GetSpoolRule).Action();

        public bool SpoolEmpty(CelloSpool spool, int amount) =>
            RuleFactory.CreateIioBusinessRule<RemoveAmountData, RemoveAmountResult>(RuleNames.SpoolEmptyRule).Action(new RemoveAmountData(spool, amount)).Ok;

        public IEnumerable<CelloSpool> AddSpool(IEnumerable<CelloSpool> spool) => RuleFactory.CreateIioBusinessRule<IEnumerable<CelloSpool>, IEnumerable<CelloSpool>>(RuleNames.AddSpoolRule)
                                                                                             .Action(spool);

        public bool AddSpoolAmount(CelloSpool spool, int amount) => RuleFactory.CreateIioBusinessRule<AddAmountData, AddAmountResult>(RuleNames.AddAmountRule)
                                                                               .Action(new AddAmountData(spool, amount)).Result;

        public void UpdateSpools(IEnumerable<CelloSpool> spools) => RuleFactory.CreateIiBusinessRule<IEnumerable<CelloSpool>>(RuleNames.UpdateSpoolsRules).Action(spools);

        public void RemoveSpool(CelloSpool spool) => RuleFactory.CreateIiBusinessRule<CelloSpool>(RuleNames.RemoveSpoolRule).Action(spool);
    }
}