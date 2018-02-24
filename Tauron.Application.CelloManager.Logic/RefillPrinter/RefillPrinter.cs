using Tauron.Application.CelloManager.Logic.Historie;
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

        public void Print(CommittedRefill refill) => RuleFactory.CreateIiBusinessRule<CommittedRefill>(RuleNames.RefillPrinterRule).Action(refill);
    }
}