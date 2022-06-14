using System.Collections.Immutable;
using EcsRx.Collections.Database;
using RaiseOfNewWorld.Engine.Data;
using RaiseOfNewWorld.Game;

namespace RaiseOfNewWorld.Engine.Rooms.Maps;

public static class DimensionMapBuilder
{
    private static ImmutableDictionary<int, ImmutableDictionary<string, RoomData>> _roomData =
        ImmutableDictionary<int, ImmutableDictionary<string, RoomData>>.Empty;

    public static void InitMap(IEntityDatabase database)
    {
        foreach (var dimesnion in _roomData)
        {
            var coll = database.CreateCollection(dimesnion.Key);
            foreach (var roomData in dimesnion.Value) 
                coll.CreateEntity(new RoomBluePrint(roomData.Value.Id, dimesnion.Key, roomData.Value.Links));
        }
    }

    public static DimensionMap CreateMap() =>
        new DimensionMap(_roomData
            .Select(p => KeyValuePair.Create(p.Key,
                new RoomMap(p.Value
                    .Select(r => KeyValuePair.Create(r.Key, r.Value.Room)).ToImmutableDictionary())))
            .ToImmutableDictionary());

    private sealed record RoomData(string Id, RoomBase Room, ImmutableArray<RoomLink> Links);

    public static void CreateWorld(ContentManager contentManager, Action<DimensionBuilder> builder)
    {
        var builderInst = new DimensionBuilder(contentManager);
        builder(builderInst);
        var data = (IDimesionBuilder)builderInst;
        data.Validate();

        _roomData = data.GetDimesions()
            .ToImmutableDictionary(
                d => d.Key,
                d => d.Value.GetRomms()
                    .ToImmutableDictionary(
                        r => r.Key,
                        r => new RoomData(r.Value.Id, r.Value.CreateRoom(), r.Value.Links.ToImmutableArray())));
    }

    public interface IDimesionBuilder
    {
        IEnumerable<KeyValuePair<int, IRoomsBuilder>> GetDimesions();

        void Validate();
    }

    public interface IRoomsBuilder
    {
        IEnumerable<KeyValuePair<string, IRoomBuilder>> GetRomms();

        void Validate();
    }

    public interface IRoomBuilder
    {
        string Id { get; }

        IEnumerable<RoomLink> Links { get; }

        RoomBase CreateRoom();

        void Validate();
    }
}

public sealed class DimensionBuilder :DimensionMapBuilder.IDimesionBuilder
{
    private readonly ContentManager _contentManager;
    private readonly Dictionary<int, DimensionMapBuilder.IRoomsBuilder> _roomsBuilders = new();

    public DimensionBuilder(ContentManager contentManager) => _contentManager = contentManager;

    public DimensionBuilder WithDimension(int dimension, Action<RoomsesBuilder> builder)
    {
        if (dimension == 0)
            throw new InvalidOperationException("Dimesion 0 is for GameState");
        
        var builderInst = new RoomsesBuilder(_contentManager);
        _roomsBuilders.Add(dimension, builderInst);
        builder(builderInst);
        return this;
    }

    IEnumerable<KeyValuePair<int, DimensionMapBuilder.IRoomsBuilder>> DimensionMapBuilder.IDimesionBuilder.GetDimesions() => _roomsBuilders;

    void DimensionMapBuilder.IDimesionBuilder.Validate()
    {
        foreach (var roomsBuilder in _roomsBuilders) roomsBuilder.Value.Validate();
    }
}

public sealed class RoomsesBuilder : DimensionMapBuilder.IRoomsBuilder
{
    private readonly ContentManager _contentManager;
    private readonly Dictionary<string, DimensionMapBuilder.IRoomBuilder> _builder = new();

    public RoomsesBuilder(ContentManager contentManager) => _contentManager = contentManager;

    public RoomsesBuilder WithRoom(string id, Action<RoomBuilder> builder)
    {
        var builderInst = new RoomBuilder(id, _contentManager);
        _builder.Add(id, builderInst);
        builder(builderInst);
        return this;
    }

    IEnumerable<KeyValuePair<string, DimensionMapBuilder.IRoomBuilder>> DimensionMapBuilder.IRoomsBuilder.GetRomms() => _builder;

    void DimensionMapBuilder.IRoomsBuilder.Validate()
    {
        foreach (var roomBuilder in _builder) roomBuilder.Value.Validate();
    }
}

public sealed class RoomBuilder : DimensionMapBuilder.IRoomBuilder
{
    private static readonly HashSet<LinkDirection> Directions = new();
    private static readonly HashSet<string> Ids = new();

    private readonly ContentManager _contentManager;
    private readonly string _id;
    private readonly List<RoomLink> _links = new();
    
    private RoomFactory? _roomFactory;

    public RoomBuilder(string id, ContentManager contentManager)
    {
        _contentManager = contentManager;
        _id = id;
    }

    public RoomBuilder WithLink(RoomLink link)
    {
        _links.Add(link);
        return this;
    }

    public RoomBuilder WithFactory(RoomFactory factory)
    {
        _roomFactory = factory;
        return this;
    }

    string DimensionMapBuilder.IRoomBuilder.Id => _id;

    IEnumerable<RoomLink> DimensionMapBuilder.IRoomBuilder.Links => _links;

    RoomBase DimensionMapBuilder.IRoomBuilder.CreateRoom()
        => _roomFactory?.Invoke(this, _contentManager) ?? throw new InvalidOperationException("No RoomFactory provided");

    void DimensionMapBuilder.IRoomBuilder.Validate()
    {
        string? error = null;

        if (_roomFactory == null)
            error = "No Room Factory";
        if (string.IsNullOrWhiteSpace(_id))
            error = "No Id Provided";

        lock (Directions)
        {
            Directions.Clear();
            Ids.Clear();

            foreach (var roomLink in _links)
            {
                if (!Ids.Add(roomLink.Target))
                    error = "Dumplicate Target";
                
                if (!Directions.Add(roomLink.LinkDirection))
                    error = "Duplicate Link Direction";
            }
        }
        
        if(string.IsNullOrWhiteSpace(error)) return;

        throw new InvalidOperationException($"Error Validate: {_id} -- {error}");
    }
}

public delegate RoomBase RoomFactory(DimensionMapBuilder.IRoomBuilder roomBuilder, ContentManager contentManager);
