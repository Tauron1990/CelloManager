using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using CelloManager.Core.Logic;
using DynamicData;
using ReactiveUI;

namespace CelloManager.ViewModels.Editing;

public sealed class EditorSpoolGroup : ViewModelBase, IDisposable
{
    private readonly IDisposable _subscription;

    public EditorSpoolGroup(string categoryName, IObservableCache<ReadySpoolModel, string> spools)
    {
        CategoryName = categoryName;
        _subscription = spools
                       .Connect()
                       .Sort(ReadySpoolSorter.ModelSorter)
                       .ObserveOn(RxApp.MainThreadScheduler).Bind(out ReadOnlyObservableCollection<ReadySpoolModel> list).Subscribe();
        Spools = list;
    }

    public string CategoryName { get; }

    public ReadOnlyObservableCollection<ReadySpoolModel> Spools { get; }

    public void Dispose() => _subscription.Dispose();
}