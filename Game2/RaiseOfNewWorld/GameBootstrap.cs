using Game.Engine;
using Game.Engine.Core;
using Game.Engine.Core.Player;
using Game.Engine.Core.Rooms.Maps;
using Game.Engine.Core.Time;
using RaiseOfNewWorld.Screens;
using Terminal.Gui;

namespace RaiseOfNewWorld;

public static class GameBootstrap
{
    public static async ValueTask InitialieGame(GameManager gameManager, IProgress<int> progress)
    {
        await gameManager.InitSystem();
        gameManager.ScreenManager.Switch(nameof(MainScreen));
    }
    
    public static async ValueTask StartNewGame(GameManager gameManager, IProgress<int> process)
    {
        await Task.Delay(TimeSpan.FromSeconds(2));
        await gameManager.ClearGame(
            () =>
            {
                var database = gameManager.Database;

                var mainCollection = database.GetCollection();

                mainCollection.CreateEntity(new TimeBlueprint(GameManager.DataManager.ContentManager));
                mainCollection.CreateEntity(new PlayerBlueprint());
                mainCollection.CreateEntity(new GameInfoBlueprint());

                DimensionMapBuilder.InitMap(database);

                gameManager.ScreenManager.Switch(
                    "GameScreen",
                    true);
            });
    }
    
    public static Func<GameManager, IProgress<int>, ValueTask> LoadGame(string gameName)
        => async (gameManager, _) =>
        {
            try
            {
                await gameManager.ClearGame(
                    () =>
                    {
                        EntityManager.Load(
                            gameManager.Database,
                            gameName);

                        gameManager.ScreenManager.Switch(
                            "GameScreen",
                            false);
                    });
            }
            catch (Exception e)
            {
                MessageBox.Query(
                    "Fehler beim Laden des Spiels",
                    e.ToString(),
                    "Ok");
                gameManager.ScreenManager.Switch(nameof(MainScreen));
            }
        };
}