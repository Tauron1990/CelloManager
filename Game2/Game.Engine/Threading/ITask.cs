namespace Game.Engine.Threading;

public interface ITask : IRunnable
{
    Task? Task { get; }
}