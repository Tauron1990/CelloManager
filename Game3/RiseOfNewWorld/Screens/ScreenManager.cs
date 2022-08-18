using Raylib_CsLo;

namespace RiseOfNewWorld.Screens;

public class ScreenManager
{
    private readonly HashSet<RenderElement>[] _elements = new HashSet<RenderElement>[100];

    public ScreenManager()
    {
        for (int i = 0; i < _elements.Length; i++)
            _elements[i] = new HashSet<RenderElement>();
    }

    public void RegisterElement(RenderElement element) 
        => _elements[element.ZIndex].Add(element);

    public void RemoveElement(RenderElement element)
        => _elements[element.ZIndex].Remove(element);

    public void ChangeZIndex(RenderElement element, int index, int oldIndex)
    {
        if(_elements[oldIndex].Remove(element))
        {
            if(_elements[index].Add(element) && element.ZIndex == index)
                return;
        }

        throw new InvalidOperationException("Element Z Index is not Correct");
    }

    public void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Raylib.LIGHTGRAY);
        
        foreach (var element in 
                 from set in _elements
                 from ele in set
                 select ele)
            element.Draw();

        Raylib.EndDrawing();
    }
}