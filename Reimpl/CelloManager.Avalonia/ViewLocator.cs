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
using SpoolGroupViewModel = CelloManager.ViewModels.SpoolDisplay.SpoolGroupViewModel;

namespace CelloManager
{
    public class ViewLocator : IDataTemplate
    {
        public IControl Build(object data)
        {
            return data switch
            {
                SpoolDisplayViewModel spoolDisplayViewModel => new SpoolDisplayView { ViewModel = spoolDisplayViewModel },
                ViewModels.SpoolDisplay.SpoolGroupViewModel groupViewModel => new SpoolGroupView { ViewModel = groupViewModel },
                EditTabViewModel editTabViewModel => new EditTabView { ViewModel = editTabViewModel },
                NewSpoolEditorViewModel newSpoolEditorViewModel => new NewSpoolEditorView { ViewModel = newSpoolEditorViewModel },
                ModifySpoolEditorViewModel modifySpoolEditorViewModel => new SpoolEditorView { ViewModel = modifySpoolEditorViewModel },
                EditSpoolGroupViewModel editSpoolGroupView => new EditSpoolGroupView { ViewModel = editSpoolGroupView },
                OrderDisplayViewModel orderDisplayViewModel => new OrderDisplayView { ViewModel = orderDisplayViewModel },
                ImportViewModel importViewModel => new ImportView { ViewModel = importViewModel },
                OrderDisplayListViewModel orderDisplayListViewModel => new OrderDisplayListView { ViewModel = orderDisplayListViewModel },
                _ => TryFindByConvertion(),
            };

            IControl TryFindByConvertion()
            {
                string name = data.GetType().FullName!.Replace("ViewModel", "View");
                var type = Type.GetType(name);

                if (type != null)
                {
                    return (Control)Activator.CreateInstance(type)!;
                }
                else
                {
                    return new TextBlock { Text = "Not Found: " + name };
                }
            }
        }

        public bool Match(object data)
        {
            return data is ViewModelBase;
        }
    }
}