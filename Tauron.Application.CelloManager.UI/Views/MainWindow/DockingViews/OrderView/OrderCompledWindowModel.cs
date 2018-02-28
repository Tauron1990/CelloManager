using System;
using System.Linq;
using JetBrains.Annotations;
using Tauron.Application.CelloManager.Logic.Historie;
using Tauron.Application.CelloManager.Resources;
using Tauron.Application.Ioc;
using Tauron.Application.Models;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews.OrderView
{
    [ExportViewModel(AppConststands.OrderCompledWindow)]
    public class OrderCompledWindowModel : ViewModelBase, IResultProvider
    {
        private readonly CommittedRefill _refill;
        private bool _checkAll;
        private bool _blockSwitch;

        [Inject]
        public OrderCompledWindowModel([NotNull]CommittedRefill refill)
        {
            _refill = refill ?? throw new ArgumentNullException(nameof(refill));

            Initialize();
        }

        public OrderCompledWindowModel()
        {
            _refill = new CommittedRefill(new []
                                         {
                                             new CommittedSpool("69", 5, "Matt", 20),
                                             new CommittedSpool("45", 3, "Glanz", 5) 
                                         }, DateTime.Now, new DateTime(), 70);

            Initialize();
        }

        public UISyncObservableCollection<OrderedSpool> Spools { get; } = new UISyncObservableCollection<OrderedSpool>();

        public bool CheckAll
        {
            get => _checkAll;
            set
            {
                _checkAll = value;
                OnPropertyChanged();
                SwitchCheck(value);
            }
        }

        [WindowTarget]
        public IWindow Window { get; set; }

        private void Initialize()
        {
            using (Spools.BlockChangedMessages())
                Spools.AddRange(_refill.CommitedSpools.Select(r => new OrderedSpool(r, OrderSpoolIsChecked)));
        }

        private void OrderSpoolIsChecked()
        {
            _blockSwitch = true;

            bool spoolsCheck = Spools.All(e => e.IsChecked);
            if (spoolsCheck && !CheckAll)
                CheckAll = true;
            if (!spoolsCheck && CheckAll)
                CheckAll = false;

            _blockSwitch = false;
        }

        private void SwitchCheck(bool value)
        {
            if(_blockSwitch) return;

            _blockSwitch = true;

            foreach (var orderedSpool in Spools)
                orderedSpool.IsChecked = value;

            _blockSwitch = false;
        }

        [CommandTarget]
        public void Commit()
        {
            if (Spools.Any(o => !o.IsChecked))
            {
                var erg = Dialogs.ShowMessageBox(Window, UIResources.OrderCompledWindowNotCheckedMessage, "Warning", MsgBoxButton.YesNo, MsgBoxImage.Warning, null);
                if (erg == MsgBoxResult.No)
                {
                    Result = false;
                    Window.DialogResult = false;
                    return;
                }
            }

            Result = true;
            Window.DialogResult = true;
        }

        public object Result { get; private set; }
    }
}