using System;
using System.Reactive.Linq;
using CelloManager.Avalonia.Core.Logic;
using ReactiveUI;
using Splat;

namespace CelloManager.Avalonia.ViewModels.Orders;

public sealed class OrderDisplayViewModel : ViewModelBase, IDisposable, ITabInfoProvider
{
    private readonly SerialDisposableSubject<ViewModelBase?> _currentContentSubject = new(null);
    private readonly ObservableAsPropertyHelper<ViewModelBase?> _currentContent;

    public ViewModelBase? CurrentContent => _currentContent.Value;

    public OrderDisplayViewModel(OrderManager manger)
    {
        _currentContent = _currentContentSubject
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, m => m.CurrentContent);
        
    }

    public string Title => "Bestellungen";
    public bool CanClose => true;

    public void Dispose()
    {
        _currentContentSubject.Dispose();
        _currentContent.Dispose();
    }
}