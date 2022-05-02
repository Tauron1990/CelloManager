using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CelloManager.Avalonia.Core.Logic;
using DynamicData;
using DynamicData.Alias;
using JetBrains.Annotations;
using ReactiveUI;

namespace CelloManager.Avalonia.ViewModels.Editing;

public sealed class EditTabViewModel : ViewModelBase, ITabInfoProvider, IActivatableViewModel
{
    private readonly BehaviorSubject<ViewModelBase?> _currentEditorModelSubject = new(null);
    private readonly ObservableAsPropertyHelper<ViewModelBase?> _currentEditorModel; 
    private object? _currentSelected;
    
    public string Title => "Beabeiten";
    
    public bool CanClose => true;

    public ReadOnlyObservableCollection<EditorSpoolGroup>? SpoolGroups { get; private set; }

    public ReactiveCommand<Unit, Unit> NewSpool { get; }

    public object? CurrentSelected
    {
        get => _currentSelected;
        set => this.RaiseAndSetIfChanged(ref _currentSelected, value);
    }

    public ViewModelBase? CurrentEditorModel => _currentEditorModel.Value;
    
    public EditTabViewModel(SpoolManager spoolManager)
    {
        this.WhenActivated(Init);
        
        NewSpool =  ReactiveCommand.Create(() => _currentEditorModelSubject.OnNext(new TODONewSpoolEditorViewModel(spoolManager)));
        
        IEnumerable<IDisposable> Init()
        {
            CurrentSelected = null;

            yield return spoolManager.CurrentSpools
                .Select(s => new EditorSpoolGroup(s.Key, s.Cache))
                .DisposeMany()
                .Bind(out var groups)
                .Subscribe();

            SpoolGroups = groups;
        }
    }

    ViewModelActivator IActivatableViewModel.Activator { get; } = new();
}

public sealed class EditorSpoolGroup : ViewModelBase, IDisposable
{
    private readonly IDisposable _subscription;
    
    public string CategoryName { get; }
    
    public ReadOnlyObservableCollection<ReadySpoolModel> Spools { get; }

    public EditorSpoolGroup(string categoryName, IObservableCache<ReadySpoolModel, string> spools)
    {
        CategoryName = categoryName;
        _subscription = spools.Connect().Bind(out var list).Subscribe();
        Spools = list;
    }

    public void Dispose() => _subscription.Dispose();
}