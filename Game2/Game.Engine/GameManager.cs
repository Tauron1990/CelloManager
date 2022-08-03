using System.Collections.Immutable;
using EcsRx.Collections.Database;
using EcsRx.Extensions;
using EcsRx.Infrastructure;
using EcsRx.Infrastructure.Ninject;
using EcsRx.Plugins.Bootstrap;
using EcsRx.Plugins.Computeds;
using EcsRx.Plugins.GroupBinding;
using EcsRx.Plugins.ReactiveSystems;
using Game.Engine.Core;
using Game.Engine.Core.Movement;
using Game.Engine.Core.Player;
using Game.Engine.Core.Rooms;
using Game.Engine.Core.Time;
using Game.Engine.Packageing;
using Game.Engine.Packageing.Files;
using Game.Engine.Packageing.ScriptHosting;
using Game.Engine.Screens;
using SystemsRx.Events;
using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Extensions;
using SystemsRx.MicroRx.Events;
using SystemsRx.Systems;

namespace Game.Engine;

public sealed class GameManager
{
    private ImmutableList<Type> _systems = ImmutableList<Type>.Empty;
    private CoreApp? _coreApp;

    public GameManager(IScreenManager screenManager)
    {
        ScreenManager = screenManager;
        DataManager = new GameDataManager(Path.GetFullPath("GameData"), this);
    }

    public IScreenManager ScreenManager { get; }

    public IEventSystem Events => _coreApp!.EventSystem;

    public IEntityDatabase Database => _coreApp!.EntityDatabase;

    public static GameDataManager DataManager { get; private set; } = null!;

    public void RegisterSystem<TSystem>()
        where TSystem : ISystem =>
        _systems = _systems.Add(typeof(TSystem));

    public async ValueTask InitSystem()
        => await DataManager.InitManager();
    
    public async ValueTask ClearGame(Action? preStartApp)
    {
        ClearGame();
        
        _coreApp = new CoreApp(
            ScreenManager,
            DataManager,
            this,
            preStartApp,
            _systems);
        
        await Task.Run(() => _coreApp.StartApplication());
    }

    public void ClearGame()
    {
        _coreApp?.StopApplication();
        _coreApp?.Container.Dispose();
        _coreApp = null;
    }

    public void ShutdownApp()
    {
        if (_coreApp is not null)
            EntityManager.Save(
                _coreApp.EntityDatabase,
                "ExitSave");
        ScreenManager.Shutdown();
    }

    public void StopApplication() => _coreApp?.StopApplication();

    private sealed class CoreApp : EcsRxApplication
    {
        private Action? _prestart;
        private readonly IEnumerable<Type> _systems;

        public CoreApp(IScreenManager screenManager, GameDataManager contentManager, GameManager gameManager,
            Action? prestart, IEnumerable<Type> systems)
        {
            _prestart = prestart;
            _systems = systems;

            Container.Bind<GameManager>(bm => bm.ToInstance(gameManager));
            Container.Bind<GameContentManager>(cm => cm.ToInstance(contentManager.ContentManager));
            Container.Bind<GameScriptManager>(cm => cm.ToInstance(contentManager.ScriptManager));
            Container.Bind<IScreenManager>(sm => sm.ToInstance(screenManager));
        }

        public override IDependencyContainer Container { get; } = new NinjectDependencyContainer();

        public override void StopApplication()
        {
            base.StopApplication();
            Container.Dispose();
        }

        protected override void ResolveApplicationDependencies()
        {
            base.ResolveApplicationDependencies();
            _prestart?.Invoke();
            _prestart = null;
        }

        protected override void LoadModules()
        {
            base.LoadModules();

            Container.Unbind<IMessageBroker>();
            Container.Bind<IMessageBroker, CastingMessageBroker>();
        }

        protected override void LoadPlugins()
        {
            RegisterPlugin(new ComputedsPlugin());
            RegisterPlugin(new GroupBindingsPlugin());
            RegisterPlugin(new ReactiveSystemsPlugin());
            RegisterPlugin(new SystemBootstrapPlugin());
            base.LoadPlugins();
        }

        protected override void BindSystems()
        {
            BindSystem<MovementManager>();
            BindSystem<RoomManager>();
            BindSystem<TimeManager>();
            BindSystem<RoomRenderer>();
            BindSystem<GameInfoManager>();

            foreach (var system in _systems)
            {
                Container.Bind(typeof(ISystem), system, new BindingConfiguration
                {
                    WithName = system.Name
                });
            }
            
            base.BindSystems();
            
            void BindSystem<T>() where T : ISystem 
                => Container.Bind<ISystem, T>((Action<BindingBuilder>)(x => x.WithName(typeof(T).Name)));
        }

        protected override void ApplicationStarted()
        {
            var gameInfo = EntityDatabase.GetCollection()
                .Where(e => e.HasComponent<GameInfo>())
                .Select(e => e.GetComponent<GameInfo>())
                .First();

            if (gameInfo.IsNewGame.Value)
            {
                EventSystem.Publish(new SwitchDimesionEvent(1));
                EventSystem.Publish(MoveToRoom.MovePlayerTo("start"));
            }
            else
            {
                var player = EntityDatabase.GetCollection()
                    .Where(e => e.HasComponent<PlayerComponent>())
                    .Select(e => e.GetComponent<MoveableComponent>())
                    .First();

                EventSystem.Publish(new SwitchDimesionEvent(gameInfo.LastDimension.Value));
                EventSystem.Publish(MoveToRoom.MovePlayerTo(player.Position.Value));
            }
        }
    }
}