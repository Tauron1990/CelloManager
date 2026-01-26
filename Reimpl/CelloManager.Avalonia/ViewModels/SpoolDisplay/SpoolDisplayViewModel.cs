using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using CelloManager.Core.Logic;
using DynamicData;
using DynamicData.Alias;
using ReactiveUI;

namespace CelloManager.ViewModels.SpoolDisplay;

public sealed class SpoolDisplayViewModel : ViewModelBase, ITabInfoProvider, IDisposable
{
    private readonly IDisposable _subscription;

    public SpoolDisplayViewModel(SpoolManager manager)
    {
        _subscription = manager.CurrentSpools
                               .Select(g => new SpoolGroupViewModel(g.Key, g.Cache))
                               .DisposeMany()
                               .Select(m => TabViewModel.Create(m, tabs: null))
                               .ObserveOn(RxApp.MainThreadScheduler)
                               .Bind(out ReadOnlyObservableCollection<TabViewModel> observableCollection)
                               .Subscribe();

        Groups = observableCollection;
    }

    public ReadOnlyObservableCollection<TabViewModel> Groups { get; }

    public void Dispose()
        => _subscription.Dispose();

    public string Title => "Rollen";
    public bool CanClose => false;

    // private static int CompareCategory(IGroup<ReadySpoolModel, string, string> x, IGroup<ReadySpoolModel, string, string> y)
    //     => ReadySpoolSorter.CategorySorter.Compare(x.Key, y.Key);
}