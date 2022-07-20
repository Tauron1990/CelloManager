using System.Runtime.CompilerServices;
using RaiseOfNewWorld.Engine.Data;
using Terminal.Gui;

namespace RaiseOfNewWorld.Engine.Rooms.Types;

public sealed class TextDisplay : RoomBase
{
    private readonly Func<IReadOnlyCollection<string>> _pagesFactory;
    private readonly Action<GameManager> _onNext;
    private readonly string? _filePath;

    private int _index;

    public TextDisplay(Func<IReadOnlyCollection<string>> pages, Action<GameManager> onNext, [CallerFilePath] string? filePath = null)
    {
        _pagesFactory = pages;
        _onNext = onNext;
        _filePath = filePath;
    }

    public override void Display(View view, GameManager gameManager)
    {
        var pages = _pagesFactory();
        View page = new()
        {
            TextAlignment = TextAlignment.Centered,
            Width = Dim.Fill(),
            Height = Dim.Percent(90),
            X = Pos.At(1),
            Y = Pos.At(1)
        };

        TextProcessor.FormatText(pages.ElementAt(0), GameManager.ContentManager, _filePath).Render(page);
        
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
                    TextProcessor.FormatText(pages.ElementAt(_index), GameManager.ContentManager, _filePath).Render(page);
            }
        }
    }
}