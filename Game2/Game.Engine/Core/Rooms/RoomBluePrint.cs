using System.Collections.Immutable;
using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Extensions;
using Game.Engine.Core.Rooms.Maps;

namespace Game.Engine.Core.Rooms;

public sealed class RoomBluePrint : IBlueprint
{
    private readonly int _dimension;
    private readonly string _id;
    private readonly ImmutableArray<RoomLink> _links;

    public RoomBluePrint(string id, int dimension, ImmutableArray<RoomLink> links)
    {
        _id = id;
        _dimension = dimension;
        _links = links;
    }

    public void Apply(IEntity entity)
    {
        entity.AddComponent(
            new RoomComponent(
                _dimension,
                _id));
        entity.AddComponent(new RoomLinkData(_links));
    }
}