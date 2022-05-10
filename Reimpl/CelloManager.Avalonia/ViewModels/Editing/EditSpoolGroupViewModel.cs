using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CelloManager.Avalonia.Core.Logic;
using JetBrains.Annotations;
using ReactiveUI;

namespace CelloManager.Avalonia.ViewModels.Editing;

public class EditSpoolGroupViewModel : ViewModelBase, IActivatableViewModel, IDisposable
{
    public static ViewModelBase Create(ReadOnlyObservableCollection<ReadySpoolModel> spools, SpoolManager manager, 
        Action<ReadySpoolModel> modelSelected)
        => new EditSpoolGroupViewModel(spools, modelSelected, manager);

    private ReadySpoolModel? _selected;
    
    public IEnumerable<ReadySpoolModel> Spools { get; }

    public ReactiveCommand<Unit, Unit> DeleteAll { get; }

    public ReadySpoolModel? Selected
    {
        get => _selected;
        [UsedImplicitly]
        set => this.RaiseAndSetIfChanged(ref _selected, value);
    }

    private EditSpoolGroupViewModel(ReadOnlyObservableCollection<ReadySpoolModel> spools, Action<ReadySpoolModel> modelSelected, 
        SpoolManager spoolManager)
    {
        Spools = spools;
        
        this.WhenActivated(Init);
        
        DeleteAll = ReactiveCommand.Create(DeleteAllImpl);
        
        IEnumerable<IDisposable> Init()
        {
            Selected = null;
            
            yield return this.WhenAnyValue(m => m.Selected).Where(m => m is not null).Subscribe(modelSelected!);
        }
        
        void DeleteAllImpl()
        {
            Task.Run(() =>
            {
                foreach (var model in spools.ToArray()) spoolManager.Delete(model);
            });
        }
    }

    public ViewModelActivator Activator { get; } = new();

    void IDisposable.Dispose()
    {
        DeleteAll.Dispose();
        GC.SuppressFinalize(this);
    }
}