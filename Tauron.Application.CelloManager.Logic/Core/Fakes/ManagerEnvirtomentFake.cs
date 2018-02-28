using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.CelloManager.Logic.RefillPrinter;

namespace Tauron.Application.CelloManager.Logic.Core.Fakes
{
    #if(DEBUG)
    public class ManagerEnvirtomentFake : IManagerEnviroment
    {
        private class SettingsFake : ISettings
        {
            public string Dns { get; set; } = "8.8.8.8";
            public string TargetEmail { get; set; } = "Tauron.ab@gmail.com";
            public RefillPrinterType PrinterType { get; set; } = RefillPrinterType.Email;
            public bool Purge { get; set; } = false;
            public int Threshold { get; set; } = 2;
            public string DefaultPrinter { get; set; } = string.Empty;
            public int MaximumSpoolHistorie { get; set; } = 256;
            public string DockingState { get; set; } = string.Empty;
            public string SpoolDataGridState { get; set; } = string.Empty;
        }

        public ISettings Settings { get; } = new SettingsFake();

        public void Save()
        {

        }

        public void Reload()
        {

        }
    }
    #endif
}