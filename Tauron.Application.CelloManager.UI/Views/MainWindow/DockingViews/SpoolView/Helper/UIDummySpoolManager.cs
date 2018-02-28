using System.Collections.Generic;
using System.Linq;
using Tauron.Application.CelloManager.Logic.Manager;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews.Helper
{
    internal sealed class UIDummySpoolManager : ISpoolManager
    {
        public IEnumerable<CelloSpool> CelloSpools { get; } = Enumerable.Empty<CelloSpool>();


        public bool SpoolEmpty(CelloSpool spool, int amount)
        {
            return false;
        }

        public IEnumerable<CelloSpool> AddSpool(IEnumerable<CelloSpool> spool)
        {
            return spool;
        }

        public bool AddSpoolAmount(CelloSpool spool, int amount)
        {
            return false;
        }

        public void UpdateSpools(IEnumerable<CelloSpool> spools)
        {
        }

        public void RemoveSpool(CelloSpool spool)
        {

        }
    }
}