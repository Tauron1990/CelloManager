using RaiseOfNewWorld.Engine.Time;

namespace RaiseOfNewWorld.Engine.Movement;

public sealed record MoveToRoom(string Id, string RoomName, TimeSpan TimeNeed) : IConsumesTime;