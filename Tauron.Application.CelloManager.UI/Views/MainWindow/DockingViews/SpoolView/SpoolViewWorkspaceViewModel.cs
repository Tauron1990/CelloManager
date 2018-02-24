using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Syncfusion.Windows.Tools.Controls;
using Tauron.Application.CelloManager.Logic.Manager;
using Tauron.Application.CelloManager.UI.Helper;
using Tauron.Application.CelloManager.UI.Models;
using Tauron.Application.Ioc;
using Tauron.Application.Models;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews
{
    [ExportViewModel(AppConststands.SpoolViewWorkspaceViewModel)]
    [NotShared]
    public class SpoolViewWorkspaceViewModel : DockingTabworkspace
    {
        private readonly ISpoolManager _manager;
        private readonly SpoolModel _model;

        public SpoolViewWorkspaceViewModel([NotNull] string type, [NotNull] ISpoolManager manager, IEnumerable<CelloSpool> spools, SpoolModel model) 
            : base(type, "SpoolTab" + type)
        {
            CanDocument = true;
            CanClose = false;
            State = DockState.Document;

            _manager = manager;
            _model = model;
            Type = type;
            Spools = new UISyncObservableCollection<UIViewSpool>();

            Spools.AddRange(spools.OrderByDescending(b => b.Name).Select(s => new UIViewSpool(s, manager, _model)));
        }

        public string Type { get; private set; }

        public UISyncObservableCollection<UIViewSpool> Spools { get; }

        public void AddSpool(CelloSpool spool)
        {
            Spools.Add(new UIViewSpool(spool, _manager, _model));
        }

        public void RemoveSpool(CelloSpool spool)
        {
            var uiSpool = Spools.FirstOrDefault(s => spool == s);
            if(uiSpool == null) return;

            uiSpool.Deattach();
            Spools.Remove(uiSpool);
        }

        public void Deattach()
        {
            foreach (var spool in Spools)
                spool.Deattach();
        }
    }
}