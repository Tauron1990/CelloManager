using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using EcsRx.MicroRx.Events;

namespace RaiseOfNewWorld.Engine;

public sealed class CastingMessageBroker : IMessageBroker, IDisposable, SystemsRx.MicroRx.Events.IMessageBroker
{
    private readonly EventLoopScheduler _scheduler = new(); 
    private readonly Subject<object?> _broker = new();

    public void Publish<T>(T? message)
        => _broker.OnNext(message);

    public IObservable<T> Receive<T>()
        => _broker!.OfType<T>().ObserveOn(_scheduler);

    public void Dispose()
    {
        _broker.OnCompleted();
        _broker.Dispose();
    }
}