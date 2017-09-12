using Tauron.Application.CelloManager.Logic.Manager;
using Tauron.Application.CelloManager.Resources;
using Tauron.Application.CelloManager.UI.PrintOrder;
using Tauron.Application.CelloManager.UI.Views.MainWindow;
using Tauron.Application.Ioc;

namespace Tauron.Application.CelloManager.UI
{
    [ExportModule]
    public sealed class UIModule : IModule
    {
        [Inject]
        public IEventAggregator EventAggregator { private get; set; }

        public int Order { get; } = 0;

        public void Initialize(CommonApplication application)
        {
            SimpleLocalize.Register(UIResources.ResourceManager, typeof(MainWindow).Assembly);

            EventAggregator.GetEvent<PrintOrderEvent, PrintOrderEventArgs>().Subscribe(PrintOrder);
        }

        private void PrintOrder(PrintOrderEventArgs obj)
        {
            obj.Ok = PrintHelper.PrintOrder(obj.Refill);
        }
    }
}