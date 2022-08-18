using Raylib_CsLo;

namespace RiseOfNewWorld.Screens;

public abstract class RenderElement
{
    public int ZIndex { get; private set; }

    public abstract Rectangle AbsolutPosition { get; }

    public void SetZIndex(ScreenManager manager, int index)
    {
        var old = ZIndex;
        ZIndex = index;
        manager.ChangeZIndex(this, index, old);
    }
    
    public abstract void Draw();
}