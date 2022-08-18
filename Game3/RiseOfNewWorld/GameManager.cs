using System.Diagnostics;
using EcsRx.Infrastructure;
using EcsRx.Infrastructure.Ninject;
using EcsRx.Plugins.Batching;
using EcsRx.Plugins.Bootstrap;
using EcsRx.Plugins.GroupBinding;
using EcsRx.Plugins.Views;
using Raylib_CsLo;
using RiseOfNewWorld.Game;
using RiseOfNewWorld.Screens;
using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Extensions;
using SystemsRx.Plugins.Computeds;
using SystemsRx.Plugins.Runtime;

namespace RiseOfNewWorld;

public sealed class GameManager : EcsRxApplication
{
    private readonly ScreenManager _screenManager = new();

    protected override void LoadModules()
    {
        base.LoadModules();
        
        Container.Bind<ScreenManager>(c => c.ToInstance(_screenManager));
    }

    protected override void LoadPlugins()
    {
        base.LoadPlugins();

        RegisterPlugin(new BatchPlugin());
        RegisterPlugin(new ComputedsPlugin());
        RegisterPlugin(new EcsRx.Plugins.Computeds.ComputedsPlugin());
        RegisterPlugin(new ViewsPlugin());
        RegisterPlugin(new GroupBindingsPlugin());
        RegisterPlugin(new SystemBootstrapPlugin());
        RegisterPlugin(new SystemRuntimePlugin());
    }

    protected override void StartSystems()
    {
        base.StartSystems();
    }

    protected override void ApplicationStarted()
    {
        Raylib.SetTargetFPS(60);
        
        int frames = 0;
        var start = Stopwatch.StartNew();
        var lastUpdate = TimeSpan.Zero;
        
        while (!Raylib.WindowShouldClose())
        {
            var currentTime = start.Elapsed;
            var update = new OnUpdate(SinceStart: start.Elapsed, SinceUpdate: currentTime - lastUpdate);

            EventSystem.Publish(update);
            _screenManager.Draw();

            lastUpdate = currentTime;
        }
        
        StopApplication();
    }

    public override IDependencyContainer Container { get; } = new NinjectDependencyContainer();
}