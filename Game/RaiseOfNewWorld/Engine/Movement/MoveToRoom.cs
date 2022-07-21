using RaiseOfNewWorld.Engine.Time;

namespace RaiseOfNewWorld.Engine.Movement;

public sealed record MoveToRoom(string Id, string RoomName, TimeSpan TimeNeed) : IConsumesTime
{
    public static MoveToRoom MovePlayerTo(string name, TimeSpan time = default) => new(
        "player",
        name,
        time);
}