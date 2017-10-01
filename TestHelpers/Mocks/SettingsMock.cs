using Tauron.Application.CelloManager.Data.Core;

namespace TestHelpers.Mocks
{
    public class SettingsMock : ISettings
    {
        public string DefaultPrinter { get; set; }
        public int MaximumSpoolHistorie { get; set; }
        public string DockingState { get; set; }
        public string SpoolDataGridState { get; set; }
    }
}