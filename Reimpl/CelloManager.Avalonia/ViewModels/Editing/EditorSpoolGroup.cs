using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using CelloManager.Core.Logic;
using DynamicData;
using ReactiveUI;

namespace CelloManager.ViewModels.Editing;

public sealed class EditorSpoolGroup : ViewModelBase, IDisposable
{
    private readonly IDisposable _subscription;
    
    public string CategoryName { get; }
    
    public ReadOnlyObservableCollection<ReadySpoolModel> Spools { get; }

    public EditorSpoolGroup(string categoryName, IObservableCache<ReadySpoolModel, string> spools)
    {
        CategoryName = categoryName;
        _subscription = spools
                       .Connect()
                       .Sort(ReadySpoolSorter.ModelSorter)
                       .ObserveOn(RxApp.MainThreadScheduler).Bind(out var list).Subscribe();
        Spools = list;
    }

    public void Dispose() => _subscription.Dispose();
}