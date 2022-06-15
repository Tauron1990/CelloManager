using Terminal.Gui;

namespace RaiseOfNewWorld.Engine.Rooms.Types;

public sealed class TextDisplay : RoomBase
{
    private readonly IReadOnlyCollection<string> _pages;
    private readonly Action<GameManager> _onNext;
    private readonly Label _page = new()
    {
        TextAlignment = TextAlignment.Centered,
        Width = Dim.Fill(),
        Height = Dim.Percent(90),
        X = Pos.At(1),
        Y = Pos.At(1)
    };

    private int _index;

    public TextDisplay(IReadOnlyCollection<string> pages, Action<GameManager> onNext)
    {
        _pages = pages;
        _onNext = onNext;

        _page.Text = pages.ElementAt(0);
    }
    
    public override void Display(View view, GameManager gameManager)
    {
        var button = new Button
        {
            Text = "Weiter",
            X = Pos.Center(),
            Y = Pos.Bottom(_page) - 1
        };

        button.Clicked += NextClicked;
        
        view.Add(_page, button);
        
        void NextClicked()
        {
            _index++;
            if (_index == _pages.Count)
                _onNext(gameManager);
            else
                _page.Text = _pages.ElementAt(_index);
        }
    }
}