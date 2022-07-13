using EcsRx.Collections.Database;
using EcsRx.Extensions;
using EcsRx.Infrastructure;
using EcsRx.Infrastructure.Ninject;
using EcsRx.Plugins.Bootstrap;
using EcsRx.Plugins.Computeds;
using EcsRx.Plugins.GroupBinding;
using EcsRx.Plugins.ReactiveSystems;
using RaiseOfNewWorld.Engine.Data;
using RaiseOfNewWorld.Engine.Movement;
using RaiseOfNewWorld.Engine.Player;
using RaiseOfNewWorld.Engine.Rooms;
using RaiseOfNewWorld.Engine.Rooms.Maps;
using RaiseOfNewWorld.Engine.Time;
using RaiseOfNewWorld.Game.Dimensions.Special.Special;
using RaiseOfNewWorld.Screens;
using RaiseOfNewWorld.Screens.GameScreens;
using SystemsRx.Events;
using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Extensions;
using SystemsRx.MicroRx.Events;
using SystemsRx.Systems;

namespace RaiseOfNewWorld.Engine;

public sealed class GameManager
{
    private CoreApp? _coreApp;
    
    public IScreenManager ScreenManager { get; }

    public IEventSystem Events => _coreApp!.EventSystem;

    public IEntityDatabase Database => _coreApp!.EntityDatabase;
    
    public ContentManager ContentManager { get; } = new FileContentManager();
    
    public GameManager(IScreenManager screenManager)
    {
        ScreenManager = screenManager;
        screenManager.Switch(nameof(MainScreen));
    }

    public async ValueTask ClearGame(Action? preStartApp)
    {
        ClearGame();

        _coreApp = new CoreApp(ScreenManager, ContentManager, this, preStartApp);
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
        if(_coreApp is not null)
            EntityManager.Save(_coreApp.EntityDatabase, "ExitSave");
        ScreenManager.Shutdown();
    }
    
    private sealed class CoreApp : EcsRxApplication
    {
        private readonly ContentManager _contentManager;
        private Action? _prestart;

        public CoreApp(IScreenManager screenManager, ContentManager contentManager, GameManager gameManager, Action? prestart)
        {
            _contentManager = contentManager;
            _prestart = prestart;

            Container.Bind<GameManager>(bm => bm.ToInstance(gameManager));
            Container.Bind<ContentManager>(cm => cm.ToInstance(contentManager));
            Container.Bind<IScreenManager>(sm => sm.ToInstance(screenManager));
        }

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
            DimensionMapBuilder.CreateWorld(
                _contentManager,
                b =>
                {
                    SpecialBuilder.InitSpecial(b);
                });
            
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
            base.BindSystems();
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

        private void BindSystem<T>() where T : ISystem
        {
            Container.Bind<ISystem, T>((Action<BindingBuilder>) (x => x.WithName(typeof (T).Name)));
        }

        public override IDependencyContainer Container { get; } = new NinjectDependencyContainer();
    }

    public void StopApplication() => _coreApp?.StopApplication();
}