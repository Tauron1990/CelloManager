using Terminal.Gui;

namespace RaiseOfNewWorld.Engine.Rooms.Types;

public sealed class TextDisplay : RoomBase
{
    private readonly IEnumerable<string> _pages;
    private readonly Action _onNext;

    public TextDisplay(IEnumerable<string> pages, Action onNext)
    {
        _pages = pages;
        _onNext = onNext;
    }
    
    public override void Display(View view)
    {
        
    }
}