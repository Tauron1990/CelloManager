using Terminal.Gui;

namespace RaiseOfNewWorld.Engine.Rooms.Types;

public sealed class TextDisplay : RoomBase
{
    private readonly Func<IReadOnlyCollection<string>> _pagesFactory;
    private readonly Action<GameManager> _onNext;

    private int _index;

    public TextDisplay(Func<IReadOnlyCollection<string>> pages, Action<GameManager> onNext)
    {
        _pagesFactory = pages;
        _onNext = onNext;
    }

    public override void Display(View view, GameManager gameManager)
    {
        var pages = _pagesFactory();
        Label page = new()
        {
            TextAlignment = TextAlignment.Centered,
            Width = Dim.Fill(),
            Height = Dim.Percent(90),
            X = Pos.At(1),
            Y = Pos.At(1)
        };

        page.Text = pages.ElementAt(0);
        
        {
            var button = new Button
            {
                Text = "Weiter",
                X = Pos.Center(),
                Y = Pos.Bottom(page) - 1
            };

            button.Clicked += NextClicked;

            view.Add(page, button);

            void NextClicked()
            {
                _index++;
                if (_index == pages.Count)
                    _onNext(gameManager);
                else
                    page.Text = pages.ElementAt(_index);
            }
        }
    }
}