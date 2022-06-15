using System.Reactive.Linq;
using RaiseOfNewWorld.Engine;
using RaiseOfNewWorld.Engine.Data;
using RaiseOfNewWorld.Engine.Rooms;
using RaiseOfNewWorld.Engine.Rooms.Maps;
using RaiseOfNewWorld.Game;
using Terminal.Gui;

namespace RaiseOfNewWorld.Screens.GameScreens;

public sealed class GameScreen : ScreenBase
{
    public override void Setup(Window container, GameManager gameManager, object? parameter)
    {
        container.Title = "Spiel";
        
        var menu = container.CreateMenuBar(CreateMainMenu);
        
        var contentView = new View
        {
            Height = Dim.Fill(),
            Width = Dim.Fill()
        };
        
        container.Add(menu, contentView);
        
        RoomRenderer.View = contentView;

        IEnumerable<UiExtensions.MenuBarItemBuilder> CreateMainMenu(UiExtensions.MenuItemFactory factory)
        {
            yield return factory.NewMenuBarItem()
                .WithLabel("_HauptMenü")
                .WithChiledMenu(CreateGameSubMenu);
            
            IEnumerable<UiExtensions.MenuItemBuilder> CreateGameSubMenu()
            {
                yield return factory.NewMenuItem()
                    .WithLabel("Hauptmenü")
                    .WithClick(o => o
                        .Select(_ => MessageBox.Query(50, 10, "Hauptmenü", "Ohne Speichern beenden?", "Ja", "Nein"))
                        .Where(r => r == 0)
                        // ReSharper disable once AsyncVoidLambda
                        .Subscribe(async _ =>
                    {
                        gameManager.ScreenManager.Switch(nameof(MainScreen));
                        await gameManager.ClearGame(null);
                    }));
                
                yield return factory.NewMenuItem()
                    .WithLabel("Beenden")
                    .WithClick(o => o.Subscribe(_ => gameManager.ShutdownApp()));
            }
        }
    }

    public override void Teardown(GameManager gameManager) => RoomRenderer.View = null;

    public static Func<GameManager, IProgress<int>, ValueTask> LoadGame(string gameName)
        => async (gameManager, _) =>
        {
            try
            {
                await gameManager.ClearGame(() =>
                {
                    EntityManager.Load(gameManager.Database, gameName);
                    
                    gameManager.ScreenManager.Switch(nameof(GameScreen), false);
                });
            }
            catch (Exception e)
            {
                MessageBox.Query("Fehler beim Laden des Spiels", e.ToString(), "Ok");
                gameManager.ScreenManager.Switch(nameof(MainScreen));
            }
        };
    
    public static async ValueTask StartNewGame(GameManager gameManager, IProgress<int> process)
    {
        await Task.Delay(TimeSpan.FromSeconds(2));
        await gameManager.ClearGame(() =>
        {

            var database = gameManager.Database;

            var mainCollection = database.GetCollection();

            mainCollection.CreateEntity(new TimeBlueprint(gameManager.ContentManager));
            mainCollection.CreateEntity(new PlayerBlueprint());
            mainCollection.CreateEntity(new GameInfoBlueprint());
            
            DimensionMapBuilder.InitMap(database);

            gameManager.ScreenManager.Switch(nameof(GameScreen), true);
        });
    }
}