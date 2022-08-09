using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Akavache;
using ReactiveUI;
using Splat;

namespace AssetManager.Models;

public sealed class AppConfiguration : ReactiveObject, IDisposable
{
    private sealed record Data(string CurrentLoaded);
    
    private readonly CompositeDisposable _disposer = new();

    private string _currentLoaded = string.Empty;
    public string CurrentLoaded
    {
        get => _currentLoaded;
        set => this.RaiseAndSetIfChanged(ref _currentLoaded, value);
    }

    [DependencyInjectionConstructor]
    public AppConfiguration(IBlobCache cache)
    {
        cache.GetObject<Data>(nameof(AppConfiguration))
            .CatchAndDisplayError()
            .NotNull()
            .Subscribe(
            d =>
            {
                CurrentLoaded = d.CurrentLoaded;
            }).DisposeWith(_disposer);


        this.WhenAny(ac => ac.CurrentLoaded, configuration => configuration.Value)
            .SelectMany(
                _ => Observable.Return(new Data(CurrentLoaded))
                    .SelectMany(d => cache.InsertObject(nameof(AppConfiguration), d))
                    .CatchAndDisplayError())
            .Subscribe()
            .DisposeWith(_disposer);
    }

    public void Dispose() => _disposer.Dispose();
}