using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace CelloManager.Avalonia.Core.Data;

public sealed class ErrorDispatcher : IDisposable
{
    private readonly Subject<Exception> _errors = new();

    public IObservable<Exception> Errors => _errors.ObserveOn(Scheduler.Default).AsObservable();

    public void Send(Exception error)
        => _errors.OnNext(error);

    public void Dispose() => _errors.Dispose();
}