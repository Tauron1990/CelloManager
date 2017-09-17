using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using Syncfusion.Windows.Tools.Controls;
using Tauron.Application.CelloManager.Logic.Manager;
using Tauron.Application.CelloManager.UI.Models;
using Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews;
using Tauron.Application.Ioc;
using Tauron.Application.Models;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow
{
    [ExportViewModel(AppConststands.MainWindowName)]
    public sealed class MainWindowViewModel : ViewModelBase
    {
        private class SpoolViewManager
        {
            private readonly Dictionary<string, int> _content;
            private readonly ISpoolManager _manager;
            private readonly ObservableCollection<DockItem> _viewWorkspace;
            private readonly Dictionary<SpoolViewWorkspaceViewModel, DockItem> _dictionary = new Dictionary<SpoolViewWorkspaceViewModel, DockItem>();

            public SpoolViewManager(ObservableCollection<DockItem> viewWorkspace, ISpoolManager manager)
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

                foreach (var newView in _content.Keys.Select(name => new SpoolViewWorkspaceViewModel(name, _manager, spools.Where(s => s.Type == name))))
                {
                    Factory.Update(newView);

                    var item = newView.GetDockItem();
                    _dictionary[newView] = item;

                    _viewWorkspace.Add(item);
                }

            }

            public void Reset()
            {
                foreach (var view in _dictionary)
                    _viewWorkspace.Remove(view.Value);

                _dictionary.Clear();
            }
        }

        private bool _refillInProgress;
        private SpoolViewManager _spoolViewManager;

        [InjectModel(UIModule.OperationContextModelName)]
        public OperationContextModel OperationContext { private get; set; }

        [Inject]
        public ISpoolManager SpoolManager { private get; set; }

        [Inject]
        public IEventAggregator Events { private get; set; }

        public UISyncObservableCollection<DockItem> Tabs { get; } = new UISyncObservableCollection<DockItem>();

        //[CommandTarget]
        //public void Options()
        //{
        //    var window = ViewManager.CreateWindow(AppConststands.OptionsWindow);
        //    window.ShowDialogAsync(CommonApplication.Current.MainWindow);
        //}

        [CommandTarget]
        public bool CanRefill()
        {
            if (OperationContext.IsOperationRunning) return false;
            return _refillInProgress || SpoolManager.IsRefillNeeded();
        }

        [CommandTarget]
        public void Refill()
        {
            _refillInProgress = true;
            SpoolManager.PrintOrder();
            _refillInProgress = false;

            InvalidateRequerySuggested();
        }

        public override void BuildCompled()
        {
            Tabs.AddRange(new[]
            {
                Factory.Object<UpdaterContainerModel>().GetDockItem(),
                Factory.Object<SpoolDataEditingViewModel>().GetDockItem()
            });

            _spoolViewManager = new SpoolViewManager(Tabs, SpoolManager);
            _spoolViewManager.Initialize();

            Events.GetEvent<ResetDatabaseEvent, EventArgs>().Subscribe(Reset);
        }

        private void Reset(EventArgs obj)
        {
            CurrentDispatcher.BeginInvoke(() =>
            {

                _spoolViewManager.Reset();
                _spoolViewManager.Initialize();

            });
        }
    }
}