using System;
using System.Collections.Specialized;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;

namespace CelloManager.Views.Controls;

public class AutoSelectTabControl
{
    private TabControl? _tab;
    private IDisposable _subscription = Disposable.Empty; 
    private readonly SerialDisposable _disposable = new();
    
    public void Init(TabControl tab)
    {
        _tab = tab;

        _subscription.Dispose();
        _subscription = tab.Events().DetachedFromLogicalTree.Subscribe(_ => _disposable.Disposable = Disposable.Empty);
            
        tab.Events().PropertyChanged
            .Where(e => e.Property == ItemsControl.ItemsSourceProperty)
            .Select(e => e.NewValue as INotifyCollectionChanged)
            .Where(e => e is not null)
            .Subscribe(NewCollection!);
    }

    private void NewCollection(INotifyCollectionChanged collection)
    {
        _disposable.Disposable =
            collection.Events()
            .CollectionChanged
            .Where(a => a.Action == NotifyCollectionChangedAction.Add && a.NewItems is not null)
            .Select(a => a.NewItems![0])
            .Where(e => e is not null)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(NewItem!);
    }

    private void NewItem(object item)
    {
        if(_tab is null) return;
        _tab.SelectedItem = item;
    }
}