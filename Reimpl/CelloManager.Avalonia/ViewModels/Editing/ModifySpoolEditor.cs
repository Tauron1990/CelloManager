using CelloManager.Avalonia.Core.Logic;

namespace CelloManager.Avalonia.ViewModels.Editing;

public class ModifySpoolEditorViewModel : SpoolEditorViewModelBase
{
    private readonly ReadySpoolModel _model;

    public static ViewModelBase Create(ReadySpoolModel model, SpoolManager manager)
        => new ModifySpoolEditorViewModel(model, manager);
    
    
    
    public ModifySpoolEditorViewModel(ReadySpoolModel model, SpoolManager manager)
        : base(manager)
    {
        _model = model;
        
    }
}