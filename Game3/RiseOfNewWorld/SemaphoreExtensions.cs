using System.Reactive.Disposables;

namespace Game.Engine;

public static class SemaphoreExtensions
{
    public static async Task<IDisposable> WaitDisposableAsync(this SemaphoreSlim semaphoreSlim)
    {
        await semaphoreSlim.WaitAsync();
        return Disposable.Create(semaphoreSlim, semaphore => semaphore.Release());
    }
}