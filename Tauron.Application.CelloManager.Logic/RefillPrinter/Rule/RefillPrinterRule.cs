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
            var settings = ManagerEnviroment.Settings;
            var document = DocumentHelper.BuildFlowDocument(input);
            bool ok = false;

            if (settings.PrinterType == RefillPrinterType.Email)
                ok = MailHelper.MailOrder(document, settings.TargetEmail, settings.Dns);
            if(ok)
                return;


            PrintHelper.PrintOrder(document, ManagerEnviroment.Settings.DefaultPrinter, s =>
                                                                                     {
                                                                                         ManagerEnviroment.Settings.DefaultPrinter = s;
                                                                                         ManagerEnviroment.Save();
                                                                                     });
        }
    }
}