using Tauron.Application.CelloManager.Logic.Historie;
using Tauron.Application.CelloManager.Logic.RefillPrinter.DTO;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.BusinessLayer;
using Tauron.Application.Ioc;

namespace Tauron.Application.CelloManager.Logic.RefillPrinter
{
    [Export(typeof(IRefillPrinter))]
    public sealed class RefillPrinter : IRefillPrinter
    {
        [InjectRuleFactory]
        public RuleFactory RuleFactory { private get; set; }

        public bool Print(CommittedRefill refill) => RuleFactory.CreateIioBusinessRule<CommittedRefill, RefillPrinterResult>(RuleNames.RefillPrinterRule).Action(refill).Result;
    }
}