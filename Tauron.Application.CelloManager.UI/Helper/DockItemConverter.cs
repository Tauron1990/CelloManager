using System.Windows;
using System.Windows.Data;
using Syncfusion.Windows.Tools.Controls;
using Tauron.Application.Converter;
using Tauron.Application.Views;

namespace Tauron.Application.CelloManager.UI.Helper
{
    public sealed class DockItemConverter : ValueConverterFactoryBase
    {
        private class Converter : ValueConverterBase<DockingTabworkspace, DockItem>
        {
            protected override DockItem Convert(DockingTabworkspace value)
            {
                return new DockItem
                {
                    SideInDockedMode = value.SideInDockedMode,
                    State = value.State,
                    Header = value.Title,
                    TargetNameInDockedMode = value.TargetNameInDockedMode,
                    CanDocument = value.CanDocument,
                    Name = value.Name,
                    CanClose = value.CanClose,
                    Content = ViewManager.Manager.CreateViewForModel(value) as FrameworkElement
                };
            }

            protected override bool CanConvertBack { get; } = true;

            protected override DockingTabworkspace ConvertBack(DockItem value)
            {
                return new FrameworkObject(value.Content).DataContext as DockingTabworkspace;
            }
        }

        protected override IValueConverter Create()
        {
            return new Converter();
        }
    }
}