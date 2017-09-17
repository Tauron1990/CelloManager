using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Syncfusion.Windows.Tools.Controls;
using Tauron.Application.CelloManager.Data.Historie;
using Tauron.Application.CelloManager.Logic.Manager;
using Tauron.Application.CelloManager.UI.Helper;
using Tauron.Application.Ioc;
using Tauron.Application.Models;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews
{
    [ExportViewModel(AppConststands.SpoolViewWorkspaceViewModel)]
    [NotShared]
    public class SpoolViewWorkspaceViewModel : DockingTabworkspace
    {
        private readonly ISpoolManager _manager;

        public SpoolViewWorkspaceViewModel([NotNull] string type, [NotNull] ISpoolManager manager, IEnumerable<CelloSpoolBase> spools) 
            : base(type, "SpoolTab" + type)
        {
            CanDocument = true;
            CanClose = false;
            State = DockState.Document;

            _manager = manager;
            Type = type;
            Spools = new UISyncObservableCollection<UIViewSpool>();

            Spools.AddRange(spools.OrderByDescending(b => b.Name).Select(s => new UIViewSpool(s, manager)));


            EventAggregator.Aggregator.GetEvent<OrderSentEvent, CommittedRefill>().Subscribe(RefillSend);
        }

        public string Type { get; private set; }

        public UISyncObservableCollection<UIViewSpool> Spools { get; }

        public void AddSpool(CelloSpoolBase spool)
        {
            Spools.Add(new UIViewSpool(spool, _manager));
        }

        public void RemoveSpool(CelloSpoolBase spool)
        {
            Spools.Remove(Spools.FirstOrDefault(s => spool == s));
        }

        private void RefillSend(CommittedRefill obj)
        {
            _manager.UpdateSpools(Spools.Select(s => s.GetAmoutUpdaterAction()));
        }
    }
}