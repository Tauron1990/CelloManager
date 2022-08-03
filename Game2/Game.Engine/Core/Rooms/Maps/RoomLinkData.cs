using System.Collections.Immutable;
using EcsRx.Components;

namespace Game.Engine.Core.Rooms.Maps;

public sealed record RoomLinkData(ImmutableArray<RoomLink> Links) : IComponent;