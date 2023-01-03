using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CelloManager.Core.Logic;
using DynamicData;
using DynamicData.Alias;
using ReactiveUI;

namespace CelloManager.ViewModels.Editing;

public sealed class EditTabViewModel : ViewModelBase, ITabInfoProvider, IActivatableViewModel, IDisposable
{
    private readonly SerialDisposableSubject<ViewModelBase?> _currentEditorModelSubject = new(null);
    private readonly ObservableAsPropertyHelper<ViewModelBase?> _currentEditorModel; 
    private object? _currentSelected;
    private ReadOnlyObservableCollection<EditorSpoolGroup>? _spoolGroups;

    public string Title => "Beabeiten";
    
    public bool CanClose => true;

    public ReadOnlyObservableCollection<EditorSpoolGroup>? SpoolGroups
    {
        get => _spoolGroups;
        private set => this.RaiseAndSetIfChanged(ref _spoolGroups, value);
    }

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
        
        NewSpool =  ReactiveCommand.Create(() => _currentEditorModelSubject.OnNext(new NewSpoolEditorViewModel(spoolManager)));
        _currentEditorModel = _currentEditorModelSubject
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, m => m.CurrentEditorModel);
        
        IEnumerable<IDisposable> Init()
        {
            CurrentSelected = null;

            yield return spoolManager.CurrentSpools
                .Select(s => new EditorSpoolGroup(s.Key, s.Cache))
                .DisposeMany()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out var groups)
                .Subscribe();

            SpoolGroups = groups;

            yield return this.WhenAny(m => m.CurrentSelected, c => c.Value)
                .Select(o => o switch
                {
                    EditorSpoolGroup group => EditSpoolGroupViewModel.Create(group.Spools, spoolManager, model => CurrentSelected = model),
                    ReadySpoolModel spoolModel => ModifySpoolEditorViewModel.Create(spoolModel, spoolManager),
                    _ => null
                })
                .Subscribe(_currentEditorModelSubject);
            
            yield return Disposable.Create(this, self => self._currentEditorModelSubject.OnNext(null));
        }
    }

    ViewModelActivator IActivatableViewModel.Activator { get; } = new();

    public void Dispose()
    {
        ((IDisposable)_currentEditorModelSubject).Dispose();
        _currentEditorModel.Dispose();
        NewSpool.Dispose();
        ((IActivatableViewModel)this).Activator.Dispose();
    }
}

public sealed class EditorSpoolGroup : ViewModelBase, IDisposable
{
    private readonly IDisposable _subscription;
    
    public string CategoryName { get; }
    
    public ReadOnlyObservableCollection<ReadySpoolModel> Spools { get; }

    public EditorSpoolGroup(string categoryName, IObservableCache<ReadySpoolModel, string> spools)
    {
        CategoryName = categoryName;
        _subscription = spools.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var list).Subscribe();
        Spools = list;
    }

    public void Dispose() => _subscription.Dispose();
}