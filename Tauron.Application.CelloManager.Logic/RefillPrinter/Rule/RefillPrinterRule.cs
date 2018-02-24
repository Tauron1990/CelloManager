using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.CelloManager.Logic.Historie;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;
using Tauron.Application.Ioc;

namespace Tauron.Application.CelloManager.Logic.RefillPrinter.Rule
{
    [ExportRule(RuleNames.RefillPrinterRule)]
    public sealed class RefillPrinterRule : IBusinessRuleBase<CommittedRefill>
    {
        [Inject]
        public IManagerEnviroment ManagerEnviroment { get; set; }

        public override void ActionImpl(CommittedRefill input)
        {
            PrintHelper.PrintOrder(input, ManagerEnviroment.Settings.DefaultPrinter);
        }
    }
}