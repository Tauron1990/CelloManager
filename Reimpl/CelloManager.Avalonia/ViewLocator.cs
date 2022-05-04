using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CelloManager.Avalonia.ViewModels;
using CelloManager.Avalonia.ViewModels.Editing;
using CelloManager.Avalonia.ViewModels.SpoolDisplay;
using CelloManager.Avalonia.Views.Editing;
using CelloManager.Avalonia.Views.SpoolDisplay;
using SpoolGroupViewModel = CelloManager.Avalonia.ViewModels.SpoolDisplay.SpoolGroupViewModel;

namespace CelloManager.Avalonia
{
    public class ViewLocator : IDataTemplate
    {
        public IControl Build(object data)
        {
            return data switch
            {
                SpoolDisplayViewModel spoolDisplayViewModel => new SpoolDisplayView { ViewModel = spoolDisplayViewModel },
                SpoolGroupViewModel groupViewModel => new SpoolGroupView { ViewModel = groupViewModel },
                EditTabViewModel editTabViewModel => new EditTabView { ViewModel = editTabViewModel },
                NewSpoolEditorViewModel newSpoolEditorViewModel => new NewSpoolEditorView { ViewModel = newSpoolEditorViewModel },
                ModifySpoolEditorViewModel modifySpoolEditorViewModel => new SpoolEditorView { ViewModel = modifySpoolEditorViewModel },
                EditSpoolGroupViewModel editSpoolGroupView => new EditSpoolGroupView { ViewModel = editSpoolGroupView },
                _ => TryFindByConvertion()
            };

            IControl TryFindByConvertion()
            {
                var name = data.GetType().FullName!.Replace("ViewModel", "View");
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