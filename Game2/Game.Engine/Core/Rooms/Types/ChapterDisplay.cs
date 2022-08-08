using Terminal.Gui;

namespace Game.Engine.Core.Rooms.Types;

public sealed class ChapterDisplay : RoomBase
{
    private readonly string _chaperName;
    private readonly Action<GameManager> _next;
    private readonly string _description;

    public ChapterDisplay(string chaperName, Action<GameManager> next, string description = "")
    {
        _chaperName = chaperName;
        _next = next;
        _description = description;
    }

    public override void Display(View view, GameManager gameManager)
    {
        var line1 = new Label
        {
            TextAlignment = TextAlignment.Centered,
            Y = 1,
            Text = Figgle.FiggleFonts.Isometric2.Render(_chaperName)
        };


        var line2 = new Label
        {
            TextAlignment = TextAlignment.Centered,
            Y = Pos.Top(line1) + 1,
            Text = _description
        };


        var next = new Button
        {
            Text = "Weiter",
            Y = Pos.Bottom(line1),
            X = Pos.Center(),
            Enabled = false
        };


        next.Clicked += () => _next(gameManager);
        
        view.Add(line1, next);

        Application.MainLoop.AddTimeout(
            TimeSpan.FromSeconds(5),
            ml =>
            {
                view.Add(line2);
                ml.AddTimeout(TimeSpan.FromSeconds(5), _ => next.Enabled = false);
                return false;
            });
    }
}