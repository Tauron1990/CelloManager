using EcsRx.Plugins.Views.ViewHandlers;

namespace RiseOfNewWorld.Screens.Renderer;

public sealed class ScreenViewRenderer<TElement> : IViewHandler
    where TElement : RenderElement, new()
{
    private readonly ScreenManager _screenManager;

    public ScreenViewRenderer(ScreenManager screenManager) => _screenManager = screenManager;

    public void DestroyView(object view) => _screenManager.RemoveElement((RenderElement)view);

    public void SetActiveState(object view, bool isActive)
    {
        if(isActive)
            _screenManager.RegisterElement((RenderElement)view);
        else
            _screenManager.RemoveElement((RenderElement)view);
    }

    public object CreateView() => new TElement();
}