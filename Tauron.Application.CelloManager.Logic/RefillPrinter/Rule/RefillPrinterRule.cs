using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.CelloManager.Logic.Historie;
using Tauron.Application.CelloManager.Logic.RefillPrinter.DTO;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;
using Tauron.Application.Ioc;

namespace Tauron.Application.CelloManager.Logic.RefillPrinter.Rule
{
    [ExportRule(RuleNames.RefillPrinterRule)]
    public sealed class RefillPrinterRule : IOBusinessRuleBase<CommittedRefill, RefillPrinterResult>
    {
        [Inject]
        public IManagerEnviroment ManagerEnviroment { get; set; }

        public override RefillPrinterResult ActionImpl(CommittedRefill input)
        {
            var settings = ManagerEnviroment.Settings;
            var document = DocumentHelper.BuildFlowDocument(input);
            bool ok = false;

            if (settings.PrinterType == RefillPrinterType.Email)
                ok = MailHelper.MailOrder(document, settings.TargetEmail, settings.Dns);
            if(ok)
                return new RefillPrinterResult(true);


            return new RefillPrinterResult(PrintHelper.PrintOrder(document, ManagerEnviroment.Settings.DefaultPrinter, s =>
                                                                                                                       {
                                                                                                                           ManagerEnviroment.Settings.DefaultPrinter = s;
                                                                                                                           ManagerEnviroment.Save();
                                                                                                                       }));
        }
    }
}