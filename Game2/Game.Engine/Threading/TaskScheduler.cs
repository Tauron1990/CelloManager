using System.Collections.Concurrent;
using System.Reactive.Concurrency;
using EcsRx.MicroRx.Disposables;
using JetBrains.Annotations;

namespace Game.Engine.Threading;

[PublicAPI]
public sealed class ReactiveCustomTaskScheduler : TaskScheduler
{
    private readonly IScheduler _scheduler;
    private readonly bool _supportInline;
    private readonly IDictionary<int, Task> _queued = new ConcurrentDictionary<int, Task>();

    public ReactiveCustomTaskScheduler(IScheduler scheduler, bool supportInline = true)
    {
        _scheduler = scheduler;
        _supportInline = supportInline;
    }

    protected override IEnumerable<Task>? GetScheduledTasks()
        => _queued.Values;

    protected override void QueueTask(Task task)
    {
        var dipo = new SingleAssignmentDisposable();
        _queued.Add(task.Id, task);
        
        dipo.Disposable = _scheduler.Schedule(task, (_, toRun) =>
        {
            TryExecuteTask(toRun);
            _queued.Remove(toRun.Id);
            dipo.Dispose();
            
            return Disposable.Empty;
        });
    }

    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
    {
        if (_supportInline && !taskWasPreviouslyQueued) return false;

        return TryExecuteTask(task);
    }
}