using EcsRx.Plugins.Views.Pooling;
using EcsRx.Plugins.Views.Systems;
using RiseOfNewWorld.Screens.Renderer;
using SystemsRx.Events;

namespace RiseOfNewWorld.Screens;

public abstract class ScreenViewResolver<TElement> : PooledViewResolverSystem 
    where TElement : RenderElement, new()
{
    private readonly ScreenManager _manager;

    public ScreenViewResolver(ScreenManager manager, IEventSystem eventSystem) : base(eventSystem) => _manager = manager;

    protected override IViewPool CreateViewPool() => new ViewPool(20, new ScreenViewRenderer<TElement>(_manager));

    protected override void OnPoolStarting()
    {
        
    }
}