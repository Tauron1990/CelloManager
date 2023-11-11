using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CelloManager.ViewModels;
using CelloManager.ViewModels.Editing;
using CelloManager.ViewModels.Importing;
using CelloManager.ViewModels.Orders;
using CelloManager.ViewModels.SpoolDisplay;
using CelloManager.Views.Editing;
using CelloManager.Views.Importing;
using CelloManager.Views.Orders;
using CelloManager.Views.SpoolDisplay;

namespace CelloManager
{
    public class ViewLocator : IDataTemplate
    {
        public Control Build(object? data)
        {
            return data switch
            {
                SpoolDisplayViewModel spoolDisplayViewModel => new SpoolDisplayView { ViewModel = spoolDisplayViewModel },
                SpoolGroupViewModel groupViewModel => new SpoolGroupView { ViewModel = groupViewModel },
                EditTabViewModel editTabViewModel => new EditTabView { ViewModel = editTabViewModel },
                NewSpoolEditorViewModel newSpoolEditorViewModel => new NewSpoolEditorView { ViewModel = newSpoolEditorViewModel },
                ModifySpoolEditorViewModel modifySpoolEditorViewModel => new SpoolEditorView { ViewModel = modifySpoolEditorViewModel },
                EditSpoolGroupViewModel editSpoolGroupView => new EditSpoolGroupView { ViewModel = editSpoolGroupView },
                OrderDisplayViewModel orderDisplayViewModel => new OrderDisplayView { ViewModel = orderDisplayViewModel },
                ImportViewModel importViewModel => new ImportView { ViewModel = importViewModel },
                OrderDisplayListViewModel orderDisplayListViewModel => new OrderDisplayListView { ViewModel = orderDisplayListViewModel },
                _ => TryFindByConvertion(),
            };

            Control TryFindByConvertion()
            {
                var name = data?.GetType().FullName?.Replace("ViewModel", "View", StringComparison.Ordinal);
                if (!string.IsNullOrWhiteSpace(name))
                {
                    var type = Type.GetType(name);

                    if (type != null)
                    {
                        return (Control)Activator.CreateInstance(type)!;
                    }
                }

                return new TextBlock { Text = "Not Found: " + name };
            }
        }

        public bool Match(object? data)
        {
            return data is ViewModelBase;
        }
    }
}