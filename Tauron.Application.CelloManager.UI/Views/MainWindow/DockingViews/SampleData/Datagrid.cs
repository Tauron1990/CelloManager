using System;
using System.Collections.Generic;
using Tauron.Application.CelloManager.Logic.Manager;
using Tauron.Application.CelloManager.UI.Views.MainWindow.SpoolView.Tabs.Helper;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews.SampleData
{
#if DEBUG
    public static class Datagrid
    {
        private class NullNodifer : ISpoolChangeNotifer
        {
            public void SpoolValueChanged(CelloSpoolBase spool)
            {
                
            }
        }

        private static readonly ISpoolChangeNotifer NullChangeNotifer = new NullNodifer();

        public static List<GuiEditSpool> Spools = new List<GuiEditSpool>
        {
            new GuiEditSpool(NullChangeNotifer, new UIDummyCelloSpool { Name = "69", Type = "Matt", Amount = 2, Neededamount = 6}),
            new GuiEditSpool(NullChangeNotifer, new UIDummyCelloSpool { Name = "30", Type = "Matt", Amount = 1, Neededamount = 6}),
            new GuiEditSpool(NullChangeNotifer, new UIDummyCelloSpool { Name = "49", Type = "Matt", Amount = 5, Neededamount = 6}),
            new GuiEditSpool(NullChangeNotifer, new UIDummyCelloSpool { Name = "69", Type = "Glanz", Amount = 3, Neededamount = 6}),
            new GuiEditSpool(NullChangeNotifer, new UIDummyCelloSpool { Name = "52", Type = "Glanz", Amount = 4, Neededamount = 6}),
            new GuiEditSpool(NullChangeNotifer, new UIDummyCelloSpool { Name = "71", Type = "Glanz", Amount = 6, Neededamount = 6}),
        };
    }
#endif
}