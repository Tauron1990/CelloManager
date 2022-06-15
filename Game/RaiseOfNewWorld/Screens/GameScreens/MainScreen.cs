using RaiseOfNewWorld.Engine;
using Terminal.Gui;

namespace RaiseOfNewWorld.Screens.GameScreens;

public sealed class MainScreen : ScreenBase
{
    public override void Setup(Window container, GameManager manager, object? parameter)
    {
        container.Title = "Hauptmenü";
        
        var title = new Label("Rise of New World")
        {
            X = Pos.Center(),
            Y = Pos.Top(container) + 3,
        };
        
       var newGame = new Button("Neues Spiel")
        {
            X = Pos.Center(),
            Y = Pos.Bottom(title) + 2
        }.OnClick(container, o => o.Subscribe(_ => manager.ScreenManager.Switch(nameof(LoadingScreen), new LoadingParameter(GameScreen.StartNewGame, -1, "Starte Spiel"))));
       
       var loadGame = new Button("Spiel Laden")
       {
           X = Pos.Center(),
           Y = Pos.Bottom(newGame) + 1
       }.OnClick(container, o => o.Subscribe(_ => manager.ScreenManager.Switch(nameof(LoadGameScreen))));
       
       var close = new Button("Beenden")
       {
           X = Pos.Center(),
           Y = Pos.Top(loadGame) + 3
       }.OnClick(container, o => o.Subscribe(_ => manager.ScreenManager.Shutdown()));
        
        container.Add(title, newGame, loadGame, close);

        if (parameter is Action action)
            action();
    }
}