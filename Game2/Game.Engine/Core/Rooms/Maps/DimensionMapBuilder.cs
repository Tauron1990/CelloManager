using System.Collections.Immutable;
using EcsRx.Collections.Database;
using Game.Engine.Packageing.Files;

namespace Game.Engine.Core.Rooms.Maps;

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
                coll.CreateEntity(
                    new RoomBluePrint(
                        roomData.Value.Id,
                        dimesnion.Key,
                        roomData.Value.Links));
        }
    }

    public static DimensionMap CreateMap() =>
        new(
            _roomData
                .Select(
                    p => KeyValuePair.Create(
                        p.Key,
                        new RoomMap(
                            p.Value
                                .Select(
                                    r => KeyValuePair.Create(
                                        r.Key,
                                        r.Value.Room)).ToImmutableDictionary())))
                .ToImmutableDictionary());

    public static void CreateDimesion(Action<DimensionBuilder> builder)
    {
        var builderInst = new DimensionBuilder(GameManager.DataManager.ContentManager);
        builder(builderInst);
        var data = (IDimesionBuilder)builderInst;
        data.Validate();
        
        ProcessDimension(data);
    }

    public static void ModifyDimension(int dimension, Action<ModifyDimesion> modify)
    {
        var mod = new ModifyDimesion(
            dimension,
            _roomData[dimension]
                .Select(rd => rd.Value)
                .Select(rd => new RoomBuilder(GameManager.DataManager.ContentManager, rd.Id, (_, _) => rd.Room, rd.Links)));


        modify(mod);
        mod.Validate();
        
        ProcessDimension(mod);
    }

    private static void ProcessDimension(IDimesionBuilder builder)
    {
        foreach (var dimesion in builder.GetDimesions()) 
            _roomData = _roomData.SetItem(dimesion.Key, UpdateRomms(dimesion.Value));

        static ImmutableDictionary<string, RoomData> UpdateRomms(IRoomsBuilder builder) =>
            builder.GetRomms()
                .ToImmutableDictionary(
                    r => r.Key,
                    r => new RoomData(
                        r.Value.Id,
                        r.Value.CreateRoom(),
                        r.Value.Links.ToImmutableArray()));
    }

    private sealed record RoomData(string Id, RoomBase Room, ImmutableArray<RoomLink> Links);

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

public sealed class ModifyDimesion : DimensionMapBuilder.IDimesionBuilder
{
    private readonly int _dimesion;
    private readonly Dictionary<string, RoomBuilder> _roomBuilders;

    public ModifyDimesion(int dimesion, IEnumerable<RoomBuilder> rooms)
    {
        _dimesion = dimesion;
        _roomBuilders = rooms.Cast<DimensionMapBuilder.IRoomBuilder>().ToDictionary(rb => rb.Id, b => (RoomBuilder)b);
    }

    public IEnumerable<KeyValuePair<int, DimensionMapBuilder.IRoomsBuilder>> GetDimesions()
    {
        yield return KeyValuePair.Create<int, DimensionMapBuilder.IRoomsBuilder>(_dimesion, new ModifyRoomsBuilder(_roomBuilders));
    }

    public void Validate()
    {
        foreach (var roomBuilder in _roomBuilders.Values.Cast<DimensionMapBuilder.IRoomBuilder>()) roomBuilder.Validate();
    }
    
    private sealed class ModifyRoomsBuilder : DimensionMapBuilder.IRoomsBuilder
    {
        private readonly Dictionary<string, RoomBuilder> _roomBuilders;

        public ModifyRoomsBuilder(Dictionary<string, RoomBuilder> roomBuilders) => _roomBuilders = roomBuilders;
        public IEnumerable<KeyValuePair<string, DimensionMapBuilder.IRoomBuilder>> GetRomms() 
            => _roomBuilders.Select(kp => KeyValuePair.Create<string, DimensionMapBuilder.IRoomBuilder>(kp.Key, kp.Value));

        public void Validate() { }
    }
}

public sealed class DimensionBuilder : DimensionMapBuilder.IDimesionBuilder
{
    private readonly GameContentManager _contentManager;
    private readonly Dictionary<int, DimensionMapBuilder.IRoomsBuilder> _roomsBuilders = new();

    public DimensionBuilder(GameContentManager contentManager) => _contentManager = contentManager;

    IEnumerable<KeyValuePair<int, DimensionMapBuilder.IRoomsBuilder>> DimensionMapBuilder.IDimesionBuilder.
        GetDimesions() => _roomsBuilders;

    void DimensionMapBuilder.IDimesionBuilder.Validate()
    {
        foreach (var roomsBuilder in _roomsBuilders) roomsBuilder.Value.Validate();
    }

    public DimensionBuilder WithDimension(int dimension, Action<RoomsesBuilder> builder)
    {
        if (dimension == 0)
            throw new InvalidOperationException("Dimesion 0 is for GameState");

        var builderInst = new RoomsesBuilder(_contentManager);
        _roomsBuilders.Add(
            dimension,
            builderInst);
        builder(builderInst);
        return this;
    }
}

public sealed class RoomsesBuilder : DimensionMapBuilder.IRoomsBuilder
{
    private readonly Dictionary<string, DimensionMapBuilder.IRoomBuilder> _builder = new();
    private readonly GameContentManager _contentManager;

    public RoomsesBuilder(GameContentManager contentManager)
    {
        _contentManager = contentManager;
    }

    IEnumerable<KeyValuePair<string, DimensionMapBuilder.IRoomBuilder>> DimensionMapBuilder.IRoomsBuilder.GetRomms()
        => _builder;

    void DimensionMapBuilder.IRoomsBuilder.Validate()
    {
        foreach (var roomBuilder in _builder) roomBuilder.Value.Validate();
    }

    public RoomsesBuilder WithRoom(string id, Action<RoomBuilder> builder)
    {
        var builderInst = new RoomBuilder(
            id,
            _contentManager);
        _builder.Add(
            id,
            builderInst);
        builder(builderInst);
        return this;
    }
}

public sealed class RoomBuilder : DimensionMapBuilder.IRoomBuilder
{
    private static readonly HashSet<LinkDirection> Directions = new();
    private static readonly HashSet<string> Ids = new();

    private readonly GameContentManager _contentManager;
    private readonly string _id;
    private readonly List<RoomLink> _links = new();

    private RoomFactory? _roomFactory;

    public RoomBuilder(string id, GameContentManager contentManager)
    {
        _contentManager = contentManager;
        _id = id;
    }

    public RoomBuilder(GameContentManager contentManager, string id, RoomFactory? roomFactory, IEnumerable<RoomLink> links)
    {
        _contentManager = contentManager;
        _id = id;
        _roomFactory = roomFactory;
        _links.AddRange(links);
    }

    string DimensionMapBuilder.IRoomBuilder.Id => _id;

    IEnumerable<RoomLink> DimensionMapBuilder.IRoomBuilder.Links => _links;

    RoomBase DimensionMapBuilder.IRoomBuilder.CreateRoom()
        => _roomFactory?.Invoke(
            this,
            _contentManager) ?? throw new InvalidOperationException("No RoomFactory provided");

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

        if (string.IsNullOrWhiteSpace(error)) return;

        throw new InvalidOperationException($"Error Validate: {_id} -- {error}");
    }

    public RoomBuilder RemoveLink(LinkDirection linkDirection)
    {
        if (_links.Any(l => l.LinkDirection == linkDirection))
            _links.Remove(_links.First(rl => rl.LinkDirection == linkDirection));
        
        return this;
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
}

public delegate RoomBase RoomFactory(DimensionMapBuilder.IRoomBuilder roomBuilder, GameContentManager contentManager);