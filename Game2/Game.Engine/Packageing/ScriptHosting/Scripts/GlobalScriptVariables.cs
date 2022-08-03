using Game.Engine.Core.Rooms.Maps;
using Game.Engine.Packageing.Files;
using Game.Engine.Screens;
using JetBrains.Annotations;
using SystemsRx.Events;
using SystemsRx.Systems;

namespace Game.Engine.Packageing.ScriptHosting.Scripts;

[PublicAPI]
public class GlobalScriptVariables
{ 
    public static IScreenManager ScreenManager { get; internal set; } = null!;

    public static void CreateDimesion(Action<DimensionBuilder> builder)
        => DimensionMapBuilder.CreateDimesion(builder);

    public static void ModifyDimension(int dimension, Action<ModifyDimesion> modify)
        => DimensionMapBuilder.ModifyDimension(dimension, modify);

    public GlobalScriptVariables(PackageScriptManager scriptManager, PackageContentManager contentManager, GameManager gameManager)
    {
        _gameManager = gameManager;
        ScriptManager = scriptManager;
        ContentManager = contentManager;
    }

    private readonly GameManager _gameManager;

    public void RegisterSystem<TSystem>() where TSystem : ISystem 
        => _gameManager.RegisterSystem<TSystem>();
    
    public PackageScriptManager ScriptManager { get; }

    public PackageContentManager ContentManager { get; }

    public IEventSystem EventSystem => _gameManager.Events;
}