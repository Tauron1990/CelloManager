using System.Windows;
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
        public double DesiredWidth { get; set; } = 90;
        public bool CanAutoHide { get; set; } = true;
        public double DesiredHeight { get; set; } = 100;

        protected DockingTabworkspace([NotNull] string title, string name = null) : base(title, name)
        {
        }

        public DockItem GetDockItem()
        {
            DockItem item = null;

            CurrentDispatcher.Invoke(() =>
            {
                item = new DockItem
                {
                    CanDrag = true,
                    CanFloat = true,
                    CanDock = true,
                    CanDragTab = true,
                    CanDragAutoHidden = true,
                    CanFloatMaximize = true,
                    
                    DesiredWidthInDockedMode = DesiredWidth,
                    DesiredWidthInFloatingMode = DesiredWidth,
                    Content = ViewManager.CreateViewForModel(this) as FrameworkElement,
                    SideInDockedMode = SideInDockedMode,
                    State = State,
                    Header = Title,
                    CanDocument = CanDocument,
                    Name = Name,
                    CanClose = CanClose,
                    CanAutoHide = CanAutoHide,
                    DesiredHeightInDockedMode = DesiredHeight,
                    DesiredHeightInFloatingMode = DesiredHeight
                };

                if (!string.IsNullOrWhiteSpace(TargetNameInDockedMode))
                    item.TargetNameInDockedMode = TargetNameInDockedMode;

                //var content = item.Content;
                //content.SetBinding(FrameworkElement.WidthProperty, new Binding("ActualWidth") { RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor)} );
            });
            
            return item;
        }
    }
}