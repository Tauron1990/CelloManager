using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using ReactiveUI;

namespace CelloManager.Avalonia.ViewModels;

public sealed class TabViewModel : ViewModelBase, IDisposable
{
    private readonly  ObservableAsPropertyHelper<bool> _canClose;
    private readonly ObservableAsPropertyHelper<string> _title;
    private ViewModelBase? _content;

    public bool CanClose => _canClose.Value;

    public string Title => _title.Value;

    public ViewModelBase? Content
    {
        get => _content;
        set => this.RaiseAndSetIfChanged(ref _content, value);
    }

    public ReactiveCommand<Unit, Unit> Close { get; }

    private TabViewModel(Action close, IObservable<string> titleProvider, IObservable<bool> canClose)
    {
        _canClose = canClose.ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, model => model.CanClose);
        Close = ReactiveCommand.Create(close, canClose);
        _title = titleProvider.ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, model => model.Title);
    }

    public void Dispose()
    {
        Close.Dispose();
        _title.Dispose();
        _canClose.Dispose();
    }

    public static TabViewModel Create(ViewModelBase viewModelBase, ISourceList<ViewModelBase>? tabs)
    {
        var title = viewModelBase switch
        {
           ITabInfoProvider provider => provider.WhenAny(tp => tp.Title, change => change.Value), 
            _ => Observable.Return("Unbekannt")
        };

        var canclose = viewModelBase switch
        {
            ITabInfoProvider provider when tabs is not null => provider.WhenAny(tp => tp.CanClose, change => change.Value),
            _ => Observable.Return(false)
        };

        return new TabViewModel(() => Task.Run(() => tabs?.Remove(viewModelBase)), title, canclose) {Content = viewModelBase};
    }
}

public interface ITabInfoProvider : IReactiveObject
{
    public string Title { get; }
    
    public bool CanClose { get; }
}

