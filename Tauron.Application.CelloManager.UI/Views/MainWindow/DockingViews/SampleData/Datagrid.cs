using System.Collections.Generic;
using Tauron.Application.CelloManager.Logic.Manager;
using Tauron.Application.CelloManager.UI.Models;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews.SampleData
{
#if DEBUG
    public static class Datagrid
    {
        public static List<GuiEditSpool> Spools = new List<GuiEditSpool>
        {
            new GuiEditSpool(new EditSpool(new CelloSpool("69", "Matt", 2, 6, -1))),
            new GuiEditSpool(new EditSpool(new CelloSpool("30", "Matt", 1, 6, -1))),
            new GuiEditSpool(new EditSpool(new CelloSpool("49", "Matt", 5, 6, -1))),
            new GuiEditSpool(new EditSpool(new CelloSpool("69", "Glanz", 3, 6, -1))),
            new GuiEditSpool(new EditSpool(new CelloSpool("52", "Glanz", 4, 6, -1))),
            new GuiEditSpool(new EditSpool(new CelloSpool("71", "Glanz", 6, 6, -1)))
        };
    }
#endif
}