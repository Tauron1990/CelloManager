using System.Collections.Generic;
using System.Linq;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.CelloManager.Logic.Historie.DTO;
using Tauron.Application.CelloManager.Logic.Manager;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.BusinessLayer;
using Tauron.Application.Ioc;

namespace Tauron.Application.CelloManager.Logic.Historie
{
    [Export(typeof(ICommittedRefillManager))]
    public sealed class CommittedRefillManager : ICommittedRefillManager
    {
        [Inject]
        public IManagerEnviroment Enviroment { private get; set; }

        [InjectRuleFactory]
        public RuleFactory RuleFactory { private get; set; }

        public IEnumerable<CommittedRefill> CommitedRefills => RuleFactory.CreateIioBusinessRule<GetCommittedRefillFlag, IEnumerable<CommittedRefill>>(RuleNames.GetCommittedRefillRule)
                                                                          .Action(new GetCommittedRefillFlag(true));

        public IEnumerable<CommittedRefill> PlacedOrders => RuleFactory.CreateIioBusinessRule<GetCommittedRefillFlag, IEnumerable<CommittedRefill>>(RuleNames.GetCommittedRefillRule)
                                                                       .Action(new GetCommittedRefillFlag(false));

        public void Purge() => RuleFactory.CreateIiBusinessRule<PurgeSettings>(RuleNames.PurgeRule).Action(new PurgeSettings(Enviroment.Settings.MaximumSpoolHistorie));

        public CommittedRefill PlaceOrder() => RuleFactory.CreateOBussinesRule<CommittedRefill>(RuleNames.PlaceOrderRule).Action();

        public void CompledRefill(CommittedRefill refill) => RuleFactory.CreateIiBusinessRule<CommittedRefill>(RuleNames.CompledRefillRule).Action(refill);

        public bool IsRefillNeeded(IEnumerable<CelloSpool> spools, IEnumerable<CommittedRefill> refills)
            => RuleFactory.CreateIioBusinessRule<IsRefillNeededInput ,IsRefillNeededResult>(RuleNames.IsRefillNeededRule).Action(new IsRefillNeededInput(spools?.ToArray(), refills?.ToArray()))
                          .Need;



        public int GetPageCount() => RuleFactory.CreateOBussinesRule<GetPageCountResult>(RuleNames.GetPageCount).Action().Count;

        public IEnumerable<CommittedRefill> GetPage(int pageCount) 
            => RuleFactory.CreateIioBusinessRule<GetPageItemsData, GetPageItemsResult>(RuleNames.GetPageItems).Action(new GetPageItemsData(pageCount)).CommittedRefill;
    }
}