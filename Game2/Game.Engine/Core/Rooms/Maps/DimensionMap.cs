using System.Collections.Immutable;

namespace Game.Engine.Core.Rooms.Maps;

public sealed class DimensionMap
{
    private readonly ImmutableDictionary<int, RoomMap> _roomMaps;

    public DimensionMap(ImmutableDictionary<int, RoomMap> roomMaps) => _roomMaps = roomMaps;

    public RoomMap GetMap(int dimesion) => _roomMaps[dimesion];
}