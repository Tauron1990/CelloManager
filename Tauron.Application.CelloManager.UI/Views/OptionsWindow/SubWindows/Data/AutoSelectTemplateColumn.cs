using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Tauron.Application.CelloManager.UI.Views.OptionsWindow.SubWindows
{
    public class AutoSelectTemplateColumn : DataGridTemplateColumn
    {
        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            var contentPresenter = editingElement as ContentPresenter;

            var editingControl = FindVisualChild<TextBox>(contentPresenter);

            editingControl?.Focus();
            editingControl?.SelectAll();
            return null;
        }

        private static TChildItem FindVisualChild<TChildItem>(DependencyObject obj)
            where TChildItem : DependencyObject
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                var item = child as TChildItem;
                if (item != null)
                    return item;

                var childOfChild = FindVisualChild<TChildItem>(child);
                if (childOfChild != null)
                    return childOfChild;
            }
            return null;
        }
    }
}