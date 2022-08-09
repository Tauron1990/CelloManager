using System;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace AssetManager.ViewModels;

public sealed class ErrorViewModel : ViewModelBase
{
    public string Message { get; }

    public ReactiveCommand<Unit, Unit> CloseCommand { get; }

    public ErrorViewModel(string message)
    {
        Message = message;
        CloseCommand = ReactiveCommand.CreateFromObservable(() =>
            Observable.Return(Unit.Default)
                .ObservOnDispatcher()
                .Select(
                    _ =>
                    {
                        DialogHost.DialogHost.Close(null);
                        return Unit.Default;
                    }));
    }

    public static ErrorViewModel Create(Exception e) => new(e.ToString());
}