﻿using System.Collections.Immutable;

namespace Game.Engine.Core.Rooms.Maps;

public sealed class RoomMap
{
    private readonly ImmutableDictionary<string, RoomBase> _rooms;

    public RoomMap(ImmutableDictionary<string, RoomBase> rooms)
    {
        _rooms = rooms;
    }

    public RoomBase LookUp(string name) => _rooms[name];
}