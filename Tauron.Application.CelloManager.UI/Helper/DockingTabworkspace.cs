using JetBrains.Annotations;
using Syncfusion.Windows.Tools.Controls;

namespace Tauron.Application.CelloManager.UI.Helper
{
    public abstract class DockingTabworkspace : TabWorkspace
    {
        public DockSide SideInDockedMode { get; set; } = DockSide.Left;
        public DockState State { get; set; } = DockState.Dock;
        public string TargetNameInDockedMode { get; set; }
        public bool CanDocument { get; set; }

        protected DockingTabworkspace([NotNull] string title, string name = null) : base(title, name)
        {
        }
    }
}