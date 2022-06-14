using EcsRx.Collections.Database;
using EcsRx.Infrastructure;
using EcsRx.Infrastructure.Ninject;
using EcsRx.Plugins.Bootstrap;
using EcsRx.Plugins.Computeds;
using EcsRx.Plugins.GroupBinding;
using EcsRx.Plugins.ReactiveSystems;
using RaiseOfNewWorld.Engine.Data;
using RaiseOfNewWorld.Engine.Movement;
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

namespace RaiseOfNewWorld.Engine;

public sealed class GameManager
{
    private readonly CoreApp _coreApp;
    
    public IScreenManager ScreenManager { get; }

    public IEventSystem Events => _coreApp.EventSystem;

    public IEntityDatabase Database => _coreApp.EntityDatabase;
    
    public ContentManager ContentManager { get; } = new();
    
    public GameManager(IScreenManager screenManager)
    {
        ScreenManager = screenManager;
        _coreApp = new CoreApp(screenManager, ContentManager);
        Task.Run(_coreApp.StartApplication);
    }

    public void ClearGame()
    {
        foreach (var databaseCollection in _coreApp.EntityDatabase.Collections.ToArray()) 
            _coreApp.EntityDatabase.RemoveCollection(databaseCollection.Id);
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

        public CoreApp(IScreenManager screenManager, ContentManager contentManager)
        {
            _screenManager = screenManager;
            _contentManager = contentManager;
        }

        public override void StopApplication()
        {
            base.StopApplication();
            Container.Dispose();
        }

        protected override void StartSystems()
        {
            this.BindAndStartSystem<MovementManager>();
            this.BindAndStartSystem<RoomManager>();
            this.BindAndStartSystem<TimeManager>();
            this.BindAndStartSystem<RoomRenderer>();
            base.StartSystems();
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
            
            base.BindSystems();
        }

        protected override void ApplicationStarted() => _screenManager.Switch(nameof(MainMenu));

        public override IDependencyContainer Container { get; } = new NinjectDependencyContainer();
    }

    public void StopApplication()
    {
        _coreApp.StopApplication();
    }
}