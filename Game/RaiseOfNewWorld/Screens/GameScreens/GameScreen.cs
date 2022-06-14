using System.Reactive.Linq;
using RaiseOfNewWorld.Engine;
using RaiseOfNewWorld.Engine.Movement;
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
        
        gameManager.Events.Publish(new GameViewEvent(contentView));
        
        if(parameter is true)
            gameManager.Events.Publish(new MoveToRoom("player", "start", TimeSpan.Zero));
        
        IEnumerable<UiExtensions.MenuBarItemBuilder> CreateMainMenu(UiExtensions.MenuItemFactory factory)
        {
            yield return factory.NewMenuBarItem()
                .WithLabel("HauptMenü")
                .WithChiledMenu(CreateGameSubMenu);
            
            IEnumerable<UiExtensions.MenuItemBuilder> CreateGameSubMenu()
            {
                yield return factory.NewMenuItem()
                    .WithLabel("Hauptmenü")
                    .WithClick(o => o
                        .Select(_ => MessageBox.Query(50, 10, "Hauptmenü", "Ohne Speichern beenden?", "Ja", "Nein"))
                        .Where(r => r == 0)
                        .Subscribe(_ =>
                    {
                        gameManager.ScreenManager.Switch(nameof(MainMenu));
                        gameManager.ClearGame();
                    }));
                
                yield return factory.NewMenuItem()
                    .WithLabel("Beenden")
                    .WithClick(o => o.Subscribe(_ => gameManager.ShutdownApp()));
            }
        }
    }

    public override void Teardown(GameManager gameManager) => gameManager.Events.Publish(new GameViewEvent(null));

    public static async Task StartNewGame(GameManager gameManager, IProgress<int> process)
    {
        await Task.Delay(TimeSpan.FromSeconds(2));
        gameManager.ClearGame();

        var database = gameManager.Database;

        var mainCollection = database.CreateCollection(CollectionIds.CommonCollection);
        
        mainCollection.CreateEntity(new TimeBlueprint(gameManager.ContentManager));
        mainCollection.CreateEntity(new PlayerBlueprint());
        
        DimensionMapBuilder.InitMap(database);
        
        gameManager.ScreenManager.Switch(nameof(GameScreen), true);
    }
}