using EcsRx.Components;

namespace RaiseOfNewWorld.Engine.Rooms;

public sealed record RoomComponent(int Dimesion, string Name) : IComponent;