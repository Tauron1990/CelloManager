using Tauron.Application.CelloManager.Logic.Manager;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews.Helper
{
    internal sealed class UIDummyCelloSpool : CelloSpoolBase
    {
        public UIDummyCelloSpool()
        {
            Name = "UIDummy";
            Type = "None";
            Amount = 3;
            Neededamount = 5;
            Id = 9999;
        }

        public override string Name { get; set; }
        public override string Type { get; set; }
        public override int Amount { get; set; }
        public override int Neededamount { get; set; }
        public override int Id { get; }

        public override void UpdateSpool()
        {
            
        }

        public override string UniquieId => "Dummy";
    }
}