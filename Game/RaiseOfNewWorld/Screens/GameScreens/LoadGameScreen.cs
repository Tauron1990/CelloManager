using System.Reactive.Linq;
using RaiseOfNewWorld.Engine;
using Terminal.Gui;

namespace RaiseOfNewWorld.Screens.GameScreens;

public sealed class LoadGameScreen : IScreen
{
    public void Setup(Window container, GameManager gameManager, object? parameter)
    {
        container.Title = "Spiel Laden";

        var files = EntityManager.GetSaveFiles().ToList();

        var saveGames = new ListView
        {
            AllowsMultipleSelection = false,
            Width = Dim.Fill(),
            Height = Dim.Percent(60),
            X = Pos.At(1),
            Y = Pos.At(1)
        };

        saveGames.SetSource(files);

        var loadButton = new Button
        {
            Text = "Spiel Laden",
            Y = Pos.AnchorEnd(3),
            X = Pos.Center() + 6
        }.OnClick(
            container,
            o => o
                .Where(_ => files.Count != 0)
                .Subscribe(
                    _ =>
                    {
                        gameManager.ScreenManager.Switch(
                            nameof(LoadingScreen),
                            new LoadingParameter(
                                GameScreen.LoadGame(files[saveGames.SelectedItem]),
                                -1,
                                "Spiel wird Geladen"));
                    }));

        var cancelButton = new Button
        {
            Text = "Abbrechen",
            Y = Pos.AnchorEnd(3),
            X = Pos.Center() - 10
        }.OnClick(
            container,
            o => o.Subscribe(_ => gameManager.ScreenManager.Switch(nameof(MainScreen))));

        container.Add(
            saveGames,
            cancelButton,
            loadButton);
    }

    public void Teardown(GameManager gameManager)
    {
    }
}