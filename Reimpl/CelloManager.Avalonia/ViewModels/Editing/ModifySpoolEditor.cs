using CelloManager.Avalonia.Core.Logic;

namespace CelloManager.Avalonia.ViewModels.Editing;

public class TODOModifySpoolEditorViewModel : ViewModelBase
{
    public static ViewModelBase Create(ReadySpoolModel model, SpoolManager manager)
        => new TODOModifySpoolEditorViewModel(model, manager);
    
    public TODOModifySpoolEditorViewModel(ReadySpoolModel model, SpoolManager manager)
    {
        
    }
}