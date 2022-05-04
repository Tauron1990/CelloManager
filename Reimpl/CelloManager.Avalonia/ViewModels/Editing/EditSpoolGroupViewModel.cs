using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using CelloManager.Avalonia.Core.Logic;
using ReactiveUI;

namespace CelloManager.Avalonia.ViewModels.Editing;

public class EditSpoolGroupViewModel : ViewModelBase, IActivatableViewModel
{
    public static ViewModelBase Create(IEnumerable<ReadySpoolModel> spools, SpoolManager manager, Action<ReadySpoolModel> modelSelected)
        => new EditSpoolGroupViewModel(spools, modelSelected);

    private ReadySpoolModel? _selected;
    
    public IEnumerable<ReadySpoolModel> Spools { get; }

    public ReadySpoolModel? Selected
    {
        get => _selected;
        set => this.RaiseAndSetIfChanged(ref _selected, value);
    }

    private EditSpoolGroupViewModel(IEnumerable<ReadySpoolModel> spools, Action<ReadySpoolModel> modelSelected)
    {
        Spools = spools;
        
        this.WhenActivated(Init);
        
        IEnumerable<IDisposable> Init()
        {
            Selected = null;
            
            yield return this.WhenAnyValue(m => m.Selected).Where(m => m is not null).Subscribe(modelSelected!);
        }
    }

    public ViewModelActivator Activator { get; } = new();
}