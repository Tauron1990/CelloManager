using Tauron.Application.CelloManager.Resources;
using Tauron.Application.CelloManager.UI.Views.MainWindow;

namespace Tauron.Application.CelloManager.UI
{
    [ExportModule]
    public sealed class UIModule : IModule
    {
        public const string OperationContextModelName = "OperationContext";
        public const string SpoolModelName = "SpoolModel";
        
        public int Order { get; } = 0;

        public void Initialize(CommonApplication application) => SimpleLocalize.Register(UIResources.ResourceManager, typeof(MainWindow).Assembly);
    }
}