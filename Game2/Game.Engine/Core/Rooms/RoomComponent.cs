using EcsRx.Components;

namespace Game.Engine.Core.Rooms;

public sealed record RoomComponent(int Dimesion, string Name) : IComponent;