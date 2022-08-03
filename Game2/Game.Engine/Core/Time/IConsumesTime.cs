namespace Game.Engine.Core.Time;

public interface IConsumesTime
{
    TimeSpan TimeNeed { get; }
}