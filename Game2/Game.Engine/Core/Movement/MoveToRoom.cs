using Game.Engine.Core.Time;

namespace Game.Engine.Core.Movement;

public sealed record MoveToRoom(string Id, string RoomName, TimeSpan TimeNeed) : IConsumesTime
{
    public static MoveToRoom MovePlayerTo(string name, TimeSpan time = default) => new(
        "player",
        name,
        time);
}