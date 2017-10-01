using System;
using System.Collections.Generic;
using System.Linq;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.CelloManager.Logic.Manager;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews.Helper
{
    internal sealed class UIDummySpoolManager : ISpoolManager
    {
        public UIDummySpoolManager()
        {
            CelloSpools = Enumerable.Empty<CelloSpoolBase>();
        }

        public IEnumerable<CelloSpoolBase> CelloSpools { get; }
        public void SpoolEmty(CelloSpoolBase spool)
        {
        }

        public void AddSpool(CelloSpoolBase spool, int value)
        {
        }

        public void PrintOrder()
        {
        }

        public bool IsRefillNeeded()
        {
            return false;
        }

        public void UpdateSpools(IEnumerable<Action<IUnitOfWork>> updater)
        {
            
        }
    }
}