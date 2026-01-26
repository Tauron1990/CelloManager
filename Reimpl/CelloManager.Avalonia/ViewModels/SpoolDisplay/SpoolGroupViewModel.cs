using System;
using System.Reactive.Linq;
using CelloManager.Core.DataOperators;
using CelloManager.Core.Logic;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace CelloManager.ViewModels.SpoolDisplay;

public sealed class SpoolGroupViewModel : ViewModelBase, ITabInfoProvider, IDisposable
{
    private readonly IDisposable _subscription;

    public SpoolGroupViewModel(string category, IConnectableCache<ReadySpoolModel, string> spools)
    {
        Title = category;

        _subscription = spools.Connect()
                              .Sort(ReadySpoolSorter.ModelSorter)
                              .ObserveOn(RxApp.MainThreadScheduler)
                              .SelectUpdate(m => new SpoolViewModel(m))
                              .DisposeMany()
                              .Bind(Spools)
                              .Subscribe();
    }


    public IObservableCollection<SpoolViewModel> Spools { get; } = new ObservableCollectionExtended<SpoolViewModel>();

    public void Dispose() => _subscription.Dispose();

    public bool CanClose => false;

    public string Title { get; }
}