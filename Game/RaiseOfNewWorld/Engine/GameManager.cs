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
using SystemsRx.Infrastructure;
using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Extensions;
using SystemsRx.MicroRx.Events;
using SystemsRx.Systems;

namespace RaiseOfNewWorld.Engine;

public sealed class GameManager
{
    private CoreApp _coreApp;
    
    public IScreenManager ScreenManager { get; }

    public IEventSystem Events => _coreApp.EventSystem;

    public IEntityDatabase Database => _coreApp.EntityDatabase;
    
    public ContentManager ContentManager { get; } = new();
    
    public GameManager(IScreenManager screenManager)
    {
        _coreApp = new CoreApp(screenManager, ContentManager, this);
        ScreenManager = screenManager;
        screenManager.Switch(nameof(MainMenu));
    }

    public async ValueTask ClearGame(Action? preStartApp)
    {
        _coreApp.StopApplication();
        _coreApp.Container.Dispose();

        if (preStartApp is not null)
        {
            _coreApp = new CoreApp(ScreenManager, ContentManager, this);
            await Task.Run(() =>
            {
                preStartApp();
                _coreApp.StartApplication();
            });
        }
    }

    public void ShutdownApp()
    {
        EntityManager.Save(_coreApp.EntityDatabase, "ExitSave");
        ScreenManager.Shutdown();
    }
    
    private sealed class CoreApp : EcsRxApplication
    {
        private readonly IScreenManager _screenManager;
        private readonly ContentManager _contentManager;

        public CoreApp(IScreenManager screenManager, ContentManager contentManager, GameManager gameManager)
        {
            _screenManager = screenManager;
            _contentManager = contentManager;
            
            Container.Bind<GameManager>(bm => bm.ToInstance(gameManager));
        }

        public override void StopApplication()
        {
            base.StopApplication();
            Container.Dispose();
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
            DimensionMapBuilder.CreateWorld(
                _contentManager,
                b =>
                {
                    SpecialBuilder.InitSpecial(b);
                });
            
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
                EventSystem.Publish(new MoveToRoom("player", "start", TimeSpan.Zero));
            }
            else
            {
                var player = EntityDatabase.GetCollection()
                    .Where(e => e.HasComponent<PlayerComponent>())
                    .Select(e => e.GetComponent<MoveableComponent>())
                    .First();
                
                EventSystem.Publish(new SwitchDimesionEvent(gameInfo.LastDimension.Value));
                EventSystem.Publish(new MoveToRoom("player", player.Position.Value, TimeSpan.Zero));
            }
        }

        private void BindSystem<T>() where T : ISystem
        {
            Container.Bind<ISystem, T>((Action<BindingBuilder>) (x => x.WithName(typeof (T).Name)));
        }

        public override IDependencyContainer Container { get; } = new NinjectDependencyContainer();
    }

    public void StopApplication()
    {
        _coreApp.StopApplication();
    }
}