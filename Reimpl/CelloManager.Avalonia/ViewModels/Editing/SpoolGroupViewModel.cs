using System.Collections.Generic;
using CelloManager.Avalonia.Core.Logic;

namespace CelloManager.Avalonia.ViewModels.Editing;

public class TODOSpoolGroupViewModel : ViewModelBase
{
    public static ViewModelBase Create(IEnumerable<ReadySpoolModel> spools, SpoolManager manager)
        => new TODOSpoolGroupViewModel(spools, manager);
    
    public TODOSpoolGroupViewModel(IEnumerable<ReadySpoolModel> spools, SpoolManager manager)
    {
        
    }
}