using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Syncfusion.Windows.Tools.Controls;
using Tauron.Application.CelloManager.Logic.Historie;
using Tauron.Application.CelloManager.Logic.Manager;
using Tauron.Application.CelloManager.Logic.RefillPrinter;
using Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews;
using Tauron.Application.Ioc;
using Tauron.Application.Models;

namespace Tauron.Application.CelloManager.UI.Models
{
    [ExportModel(UIModule.SpoolModelName)]
    public class SpoolModel : ModelBase
    {
        #region View

        private class SpoolViewManager
        {
            private readonly Dictionary<string, int>                           _content;
            private readonly ISpoolManager                                     _manager;
            private readonly SpoolModel                                        _model;
            private readonly ObservableCollection<DockItem>                    _viewWorkspace;
            private readonly Dictionary<SpoolViewWorkspaceViewModel, DockItem> _dictionary = new Dictionary<SpoolViewWorkspaceViewModel, DockItem>();

            public SpoolViewManager(ObservableCollection<DockItem> viewWorkspace, ISpoolManager manager, SpoolModel model)
            {
                _viewWorkspace = viewWorkspace;
                _manager       = manager;
                _model         = model;
                _content       = new Dictionary<string, int>();
            }

            public void Initialize(IEnumerable<CelloSpool> orgSpools)
            {
                List<CelloSpool> spools = new List<CelloSpool>();

                foreach (var spool in orgSpools)
                {
                    if (!_content.ContainsKey(spool.Type))
                        _content.Add(spool.Type, 0);

                    _content[spool.Type] += 1;

                    spools.Add(spool);
                }

                foreach (var newView in _content.Keys.Select(name => new SpoolViewWorkspaceViewModel(name, _manager, spools.Where(s => s.Type == name), _model)))
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
                {
                    view.Key.Deattach();
                    _viewWorkspace.Remove(view.Value);
                }

                _dictionary.Clear();
            }
        }

        private SpoolViewManager _spoolViewManager;

        private bool _valueChanged;

        private bool? _baseValue;
        private bool  _isInEditing;

        [Inject]
        public IRefillPrinter RefillPrinter { get; set; }

        [Inject]
        public ICommittedRefillManager CommittedRefillManager { get; set; }

        [Inject]
        public ISpoolManager SpoolManager { get; set; }

        public UISyncObservableCollection<DockItem> Views { get; } = new UISyncObservableCollection<DockItem>();

        public UISyncObservableCollection<CelloSpool> Spools { get; } = new UISyncObservableCollection<CelloSpool>();

        public UISyncObservableCollection<CommittedRefill> Orders { get; } = new UISyncObservableCollection<CommittedRefill>();

        public bool IsInEditing
        {
            get => _isInEditing;
            set => SetProperty(ref _isInEditing, value);
        }

        public override void BuildCompled()
        {
            foreach (var order in CommittedRefillManager.PlacedOrders)
                Orders.Add(order);

            Spools.AddRange(SpoolManager.CelloSpools);

            _spoolViewManager = new SpoolViewManager(Views, SpoolManager, this);
            _spoolViewManager.Initialize(Spools);
        }

        public bool IsRefillNeeded()
        {
            if (_valueChanged) return true;

            if (_baseValue == null)
                _baseValue = CommittedRefillManager.IsRefillNeeded();

            return _baseValue == true;
        }

        public void PlaceOrder()
        {
            var order = CommittedRefillManager.PlaceOrder();
            RefillPrinter.Print(order);
        }

        public void PrintOrder(CommittedRefill refill) => RefillPrinter.Print(refill);

        public void RefillCompled(CommittedRefill refill)
        {
            CommittedRefillManager.CompledRefill(refill);
            Orders.Remove(refill);

            foreach (var entry in refill.CommitedSpools.Select(s => new { CS = s.OrderedCount, VS = Spools.Single(ss => ss.Id == s.SpoolId) }))
            {
                entry.VS.Amount -= entry.CS;
            }

            _valueChanged = false;
            _baseValue    = null;
        }

        public void ValueChanged() => _valueChanged = true;

        #endregion

        #region Edit
        
        private List<EditSpool> _editorSpools = new List<EditSpool>();
        public IReadOnlyCollection<EditSpool> EditorSpools { get { return _editorSpools.Where(e => !e.IsDeleted).ToArray(); } }

        public void EnterEditMode()
        {
            IsInEditing = true;
            _spoolViewManager.Reset();

            _editorSpools.AddRange(Spools.Select(s => new EditSpool(s)));
        }

        public void ExitEditMode(bool apply)
        {
            if (apply)
            {
                using (Spools.BlockChangedMessages())
                {
                    foreach (var spool in _editorSpools.Where(s => !s.IsNew && s.IsDeleted))
                    {
                        SpoolManager.RemoveSpool(spool.Spool);
                        Spools.Remove(spool.Spool);
                    }


                    foreach (var group in EditorSpools.Where(e => !e.IsDeleted).GroupBy(s => s.IsNew))
                    {
                        if (group.Key)
                        {
                            foreach (var editSpool in group)
                                Spools.Add(SpoolManager.AddSpool(editSpool.Spool));
                        }
                        else
                            SpoolManager.UpdateSpools(group.Select(e => e.Spool));
                    }
                }
            }
            
            _editorSpools.Clear();
            _spoolViewManager.Initialize(Spools);

            IsInEditing = false;
        }

        public void Add(EditSpool spool)
        {
            spool.IsNew = true;
            _editorSpools.Add(spool);
        }

        public void Remove(EditSpool spool)
        {
            if (_editorSpools.Contains(spool))
                spool.IsDeleted = true;
        }

        #endregion

        public void OrderCompled(CommittedRefill selectedRefill) => CommittedRefillManager.CompledRefill(selectedRefill);
    }
}