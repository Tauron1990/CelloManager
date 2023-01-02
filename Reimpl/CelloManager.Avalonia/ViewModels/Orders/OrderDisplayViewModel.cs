using System;
using System.Reactive.Linq;
using CelloManager.Avalonia.Core.Logic;
using ReactiveUI;

namespace CelloManager.Avalonia.ViewModels.Orders;

public sealed class OrderDisplayViewModel : ViewModelBase, IDisposable, ITabInfoProvider
{
    //private readonly SerialDisposableSubject<ViewModelBase?> _currentContentSubject = new(null);
    //private readonly ObservableAsPropertyHelper<ViewModelBase?> _currentContent;

    public string Title => "Bestellungen";
    public bool CanClose => true;
    
    //public ViewModelBase? CurrentContent => _currentContent.Value;
    public OrderDisplayListViewModel CurrentContent { get; }
    
    public OrderDisplayViewModel(OrderManager manger)
    {
        // _currentContent = _currentContentSubject
        //     .ObserveOn(RxApp.MainThreadScheduler)
        //     .ToProperty(this, m => m.CurrentContent);
        //
        // _currentContentSubject.OnNext(new OrderDisplayListViewModel(manger.Orders));

        CurrentContent = new OrderDisplayListViewModel(manger.Orders);
    }

    public void Dispose()
    {
        //_currentContentSubject.Dispose();
        //_currentContent.Dispose();
    }
}