using Tauron.Application.CelloManager.Logic.Manager;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.SpoolView.Tabs.Helper
{
    internal sealed class UIDummyCelloSpool : CelloSpoolBase
    {
        public UIDummyCelloSpool()
        {
            Name = "UIDummy";
            Type = "None";
            Amount = 3;
            Neededamount = 5;
        }

        public override string Name { get; set; }
        public override string Type { get; set; }
        public override int Amount { get; set; }
        public override int Neededamount { get; set; }
        public override void UpdateSpool()
        {
            
        }

        public override string UniquieId => "Dummy";
    }
}