using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using CelloManager.Avalonia.Core.Logic;
using DynamicData;
using DynamicData.Alias;
using DynamicData.Binding;
using ReactiveUI;

namespace CelloManager.Avalonia.ViewModels.SpoolDisplay;

public sealed class SpoolDisplayViewModel : ViewModelBase, ITabInfoProvider, IDisposable
{
    private readonly IDisposable _subscription;
    
    public string Title => "Rollen";
    public bool CanClose => false;

    public ReadOnlyObservableCollection<TabViewModel> Groups { get; }

    public SpoolDisplayViewModel(SpoolManager manager)
    {
        _subscription = manager.CurrentSpools
            .Select(g => new SpoolGroupViewModel(g.Key, g.Cache))
            .DisposeMany()
            .Select(m => TabViewModel.Create(m, null))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out var observableCollection)
            .Subscribe();

        Groups = observableCollection;
    }

    public void Dispose() 
        => _subscription.Dispose();
}