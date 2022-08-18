namespace RiseOfNewWorld.Screens.Renderer.UI;

public abstract class UIElement : RenderElement
{
    public Pos X { get; set; } = 0;

    public Pos Y { get; set; } = 0;

    public Dim? Width { get; set; }

    public Dim? Height { get; set; }
    
    
}