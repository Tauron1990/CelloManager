using System;
using CelloManager.Core.Logic;

namespace CelloManager.ViewModels.Orders;

public sealed class OrderDisplayViewModel(OrderManager manger, IServiceProvider serviceProvider, MainWindowViewModel model)
    : ViewModelBase, IDisposable, ITabInfoProvider
{
    // _currentContent = _currentContentSubject
    //     .ObserveOn(RxApp.MainThreadScheduler)
    //     .ToProperty(this, m => m.CurrentContent);
    //
    // _currentContentSubject.OnNext(new OrderDisplayListViewModel(manger.Orders));

    //public ViewModelBase? CurrentContent => _currentContent.Value;
    public OrderDisplayListViewModel CurrentContent { get; } = new(manger.Orders, serviceProvider, manger, model);

    public void Dispose()
    {
        //_currentContentSubject.Dispose();
        //_currentContent.Dispose();
    }
    //private readonly SerialDisposableSubject<ViewModelBase?> _currentContentSubject = new(null);
    //private readonly ObservableAsPropertyHelper<ViewModelBase?> _currentContent;

    public string Title => "Bestellungen";
    public bool CanClose => true;
}