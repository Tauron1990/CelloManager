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

        public CelloSpool AddSpool(CelloSpool spool) => RuleFactory.CreateIioBusinessRule<CelloSpool, CelloSpool>(RuleNames.AddSpoolRule).Action(spool);

        public void AddSpoolAmount(CelloSpool spool, int amount) => RuleFactory.CreateIiBusinessRule<AddAmountData>(RuleNames.AddAmountRule).Action(new AddAmountData(spool, amount));

        public void UpdateSpools(IEnumerable<CelloSpool> spools) => RuleFactory.CreateIiBusinessRule<IEnumerable<CelloSpool>>(RuleNames.UpdateSpoolsRules).Action(spools);

        public void RemoveSpool(CelloSpool spool) => RuleFactory.CreateIiBusinessRule<CelloSpool>(RuleNames.RemoveSpoolRule).Action(spool);
    }
}