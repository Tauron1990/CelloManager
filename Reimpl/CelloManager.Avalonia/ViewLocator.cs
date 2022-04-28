using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CelloManager.Avalonia.ViewModels;
using CelloManager.Avalonia.ViewModels.SpoolDisplay;
using CelloManager.Avalonia.Views.SpoolDisplay;

namespace CelloManager.Avalonia
{
    public class ViewLocator : IDataTemplate
    {
        public IControl Build(object data)
        {
            switch (data)
            {
                case SpoolDisplayViewModel spoolDisplayViewModel:
                    return new SpoolDisplayView { ViewModel = spoolDisplayViewModel };
            }
            
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

        public bool Match(object data)
        {
            return data is ViewModelBase;
        }
    }
}