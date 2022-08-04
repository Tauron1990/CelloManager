        CreateDimesion(
            builder =>
            {
                builder.WithDimension(
                    1,
                    b =>
                    {
                        b.WithRoom(
                            "start",
                            r =>
                            {
                                r.WithFactory(
                                    (_, manager) => new TextDisplay(
                                        () => manager.GetFileAsString("prologtext").Split("@@@"),
                                        static m =>
                                        {
                                            var gameInfo = m.Database.GetCollection()
                                                .Where(e => e.HasComponent(typeof(GameInfo)))
                                                .Select(e => e.GetComponent<GameInfo>())
                                                .First();


                                            gameInfo.IsNewGame.Value = false;

                                            m.Events.Publish(MoveToRoom.MovePlayerTo("end"));
                                        }));
                            });


                        b.WithRoom(
                            "end",
                            rb =>
                            {
                                rb.WithFactory(
                                    (_, manager) => new TextDisplay(
                                        () => new[]{ manager.GetFileAsString("epilog") },
                                        static m => m.ScreenManager.Switch(
                                            "MainScreen",
                                            new Action(m.ClearGame))));
                            });
                    });
            });


        return Unit.Default;