        ScreenManager.RegisterScreen(nameof(GameScreen), new GameScreen());
        
        return Unit.Default;

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


            container.Add(
                menu,
                contentView);


            RoomRenderer.View = contentView;

            IEnumerable<UiExtensions.MenuBarItemBuilder> CreateMainMenu(UiExtensions.MenuItemFactory factory)
            {
                yield return factory.NewMenuBarItem()
                    .WithLabel("_HauptMenü")
                    .WithChiledMenu(CreateGameSubMenu);

                foreach (var builder in GameManager.DataManager.MainMenuRegistrations.SelectMany(fac => fac(factory)))
                    yield return builder;


                IEnumerable<UiExtensions.MenuItemBuilder> CreateGameSubMenu()
                {
                    yield return factory.NewMenuItem()
                        .WithLabel("Hauptmenü")
                        .WithClick(
                            o => o
                                .Select(
                                    _ => MessageBox.Query(
                                        50,
                                        10,
                                        "Hauptmenü",
                                        "Ohne Speichern beenden?",
                                        "Ja",
                                        "Nein"))
                                .Where(r => r == 0)
                                // ReSharper disable once AsyncVoidLambda
                                .Subscribe(
                                    async _ =>
                                    {
                                        gameManager.ScreenManager.Switch("MainScreen");
                                        await gameManager.ClearGame(null);
                                    }));


                    yield return factory.NewMenuItem()
                        .WithLabel("Beenden")
                        .WithClick(o => o.Subscribe(_ => gameManager.ShutdownApp()));
                }
            }
        }

        public override void Teardown(GameManager gameManager) => RoomRenderer.View = null;

    }