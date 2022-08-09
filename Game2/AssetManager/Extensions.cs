using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using AssetManager.ViewModels;
using ReactiveUI;

namespace AssetManager;

public static class Extensions
{
    public static IObservable<T?> CatchAndDisplayError<T>(this IObservable<T> input)
        => input.Catch<T?, Exception>(e => DialogHost.DialogHost.Show(ErrorViewModel.Create(e)).ToObservable().Select(_ => default(T)));

    public static void DisplayError(this Exception exception)
        => DialogHost.DialogHost.Show(ErrorViewModel.Create(exception));
    
    public static IObservable<T> ObservOnDispatcher<T>(this IObservable<T> input)
        => input.ObserveOn(RxApp.MainThreadScheduler);

    public static IObservable<T> NotNull<T>(this IObservable<T?> input)
        => input.Where(t => t is not null)!;
}