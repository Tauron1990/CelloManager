using System.Collections.Immutable;
using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Extensions;
using RaiseOfNewWorld.Engine.Rooms;
using RaiseOfNewWorld.Engine.Rooms.Maps;

namespace RaiseOfNewWorld.Game;

public sealed class RoomBluePrint : IBlueprint
{
    private readonly string _id;
    private readonly int _dimension;
    private readonly ImmutableArray<RoomLink> _links;

    public RoomBluePrint(string id, int dimension, ImmutableArray<RoomLink> links)
    {
        _id = id;
        _dimension = dimension;
        _links = links;
    }
    
    public void Apply(IEntity entity)
    {
        entity.AddComponent(new RoomComponent(_dimension, _id));
        entity.AddComponent(new RoomLinkData(_links));
    }
}