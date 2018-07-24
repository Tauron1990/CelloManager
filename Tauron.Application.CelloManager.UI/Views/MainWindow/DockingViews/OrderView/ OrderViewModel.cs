using System.Collections.Specialized;
using System.Linq;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Controls.DataPager;
using Syncfusion.Windows.Tools.Controls;
using Tauron.Application.CelloManager.Logic.Historie;
using Tauron.Application.CelloManager.Resources;
using Tauron.Application.CelloManager.UI.Helper;
using Tauron.Application.CelloManager.UI.Models;
using Tauron.Application.Ioc;
using Tauron.Application.Models;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews.OrderView
{
    [ExportViewModel(AppConststands.OrderView)]
    public class OrderViewModel : DockingTabworkspace
    {
        [InjectModel(UIModule.OperationContextModelName)]
        public OperationContextModel OperationContext { private get; set; }

        [InjectModel(UIModule.SpoolModelName)]
        public SpoolModel SpoolModel { get; set; }

        [Inject]
        public ICommittedRefillManager CommittedRefillManager { get; set; }

        [EventTarget(Synchronize = true)]
        public void ListClick()
        {
            if (SelectedRefill == null) return;

            var window = ViewManager.CreateWindow(AppConststands.OrderCompledWindow, SelectedRefill);

            window.ShowDialogAsync(MainWindow).ContinueWith(t =>
            {
                if (window.Result != null && (bool) window.Result)
                    SpoolModel.RefillCompled(SelectedRefill);
            });
        }

        public CommittedRefill SelectedRefill { get; set; }

        public OrderViewModel() : base(UIResources.OrderViewTitle, AppConststands.OrderView)
        {
            SideInDockedMode = DockSide.Bottom;
            State = DockState.AutoHidden;
            DesiredHeight = 300;
            CanClose = true;
            CanDocument = true;
            CanAutoHide = true;
        }

        private bool _refillInProgress;
        private int _pageCount;
        private PagedCollectionView _pagedSource;

        [CommandTarget]
        public bool CanRefill()
        {
            if (OperationContext.IsOperationRunning) return false;
            return _refillInProgress || SpoolModel.IsRefillNeeded();
        }

        [CommandTarget]
        public void Refill()
        {
            _refillInProgress = true;
            SpoolModel.PlaceOrder();
            _refillInProgress = false;

            InvalidateRequerySuggested();
        }

        [CommandTarget]
        public void Print() => SpoolModel.PrintOrder(SelectedRefill);

        [CommandTarget]
        public bool CanPrint() => SelectedRefill != null;

        public int PageCount
        {
            get => _pageCount;
            set => SetProperty(ref _pageCount, value);
        }

        [CommandTarget]
        public void Reload() => PageCount = CommittedRefillManager.GetPageCount();

        public PagedCollectionView PagedSource
        {
            get => _pagedSource;
            set
            {
                if (_pagedSource != null)
                    _pagedSource.CollectionChanged -= PagedSourceOnCollectionChanged;
                _pagedSource = value;
                if (_pagedSource != null)
                    _pagedSource.CollectionChanged += PagedSourceOnCollectionChanged;

            }
        }

        public UISyncObservableCollection<CommittedRefill> CommittedRefills { get; } = new UISyncObservableCollection<CommittedRefill>();

        private void PagedSourceOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            var temp = PagedSource.Cast<RecordEntry>().Select(e => (CommittedRefill) e.Data);

            using (CommittedRefills.BlockChangedMessages())
            {
                CommittedRefills.Clear();
                CommittedRefills.AddRange(temp);
            }
        }

        public void OnDemandLoading(SfDataPager sender, OnDemandLoadingEventArgs e)
        {
            var page = CommittedRefillManager.GetPage(e.StartIndex);

            var committedRefills = page as CommittedRefill[] ?? page.ToArray();
            if (!committedRefills.Any()) return;

            sender.LoadDynamicItems(e.StartIndex, committedRefills);
        }

        public override void BuildCompled()
        {
            Reload();
            base.BuildCompled();
        }
    }
}