using System.Collections.Immutable;
using EcsRx.Components;

namespace RaiseOfNewWorld.Engine.Rooms.Maps;

public sealed record RoomLinkData(ImmutableArray<RoomLink> Links) : IComponent;