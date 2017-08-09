using System;
using System.Collections.Generic;
using System.Linq;
using Tauron.Application.CelloManager.Logic.Manager;
using Tauron.Application.CelloManager.UI.Views.MainWindow.SpoolView.Tabs;
using Tauron.Application.Ioc;
using Tauron.Application.Models;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.SpoolView
{
    [ExportViewModel(AppConststands.SpoolView)]
    public sealed class SpoolViewModel : ViewModelBase, IWorkspaceHolder
    {
        private class SpoolViewManager
        {
            private readonly Dictionary<string, int> _content;
            private readonly ISpoolManager _manager;
            private readonly WorkspaceManager<SpoolViewWorkspaceViewModel> _viewWorkspace;

            public SpoolViewManager(WorkspaceManager<SpoolViewWorkspaceViewModel> viewWorkspace, ISpoolManager manager)
            {
                _viewWorkspace = viewWorkspace;
                _manager = manager;
                _content = new Dictionary<string, int>();
            }

            public void Initialize()
            {
                List<CelloSpoolBase> spools = new List<CelloSpoolBase>();

                foreach (var spool in _manager.CelloSpools)
                {
                    if (!_content.ContainsKey(spool.Type))
                        _content.Add(spool.Type, 0);

                    _content[spool.Type] += 1;

                    spools.Add(spool);
                }

                foreach (var name in _content.Keys)
                    _viewWorkspace.Add(new SpoolViewWorkspaceViewModel(name, _manager, spools.Where(s => s.Type == name)));

            }

            public void Reset()
            {
                _content.Clear();
            }

/*            private void ManagerOnDatabaseChanged(object sender, DatabaseChangedEventArgs databaseChangedEventArgs)
            {
                foreach (var addedSpool in databaseChangedEventArgs.AddedSpools)
                {
                    if (_content.ContainsKey(addedSpool.Type))
                    {
                        _viewWorkspace.First(m => m.Type == addedSpool.Type).AddSpool(addedSpool);
                        _content[addedSpool.Type] = 0;
                    }
                    else
                        _viewWorkspace.Add(new SpoolViewWorkspaceViewModel(addedSpool.Type, _manager));

                    _content[addedSpool.Type] = _content[addedSpool.Type] + 1;
                }

                foreach (var removedSpool in databaseChangedEventArgs.RemovedSpools)
                {
                    if(!_content.ContainsKey(removedSpool.Type)) return;

                    int value = _content[removedSpool.Type];
                    value--;
                    _content[removedSpool.Type] = value;

                    var model = _viewWorkspace.First(m => m.Type == removedSpool.Type);

                    if (value <= 0)
                    {
                        _viewWorkspace.Remove(model);
                        _content.Remove(removedSpool.Type);
                    }
                    else
                        model.RemoveSpool(removedSpool);
                }
            }*/
        }

        [Inject]
        private IEventAggregator _events;

        private SpoolViewManager _spoolViewManager;

        public SpoolViewModel()
        {
            SpoolTypes = new WorkspaceManager<SpoolViewWorkspaceViewModel>(this);
        }

        [Inject]
        public ISpoolManager Manager { get; set; }

        public WorkspaceManager<SpoolViewWorkspaceViewModel> SpoolTypes { get; private set; }

        public void Register(ITabWorkspace workspace)
        {
        }

        public void UnRegister(ITabWorkspace workspace)
        {
        }

        public override void BuildCompled()
        {
            _spoolViewManager = new SpoolViewManager(SpoolTypes, Manager);
            _spoolViewManager.Initialize();

            _events.GetEvent<ResetDatabaseEvent, EventArgs>().Subscribe(Reset);
        }

        private void Reset(EventArgs obj)
        {
            CurrentDispatcher.BeginInvoke(() =>
            {
                using (SpoolTypes.BlockChangedMessages())
                {
                    SpoolTypes.Clear();
                    _spoolViewManager.Reset();
                    _spoolViewManager.Initialize();
                }
            });
        }
    }
}