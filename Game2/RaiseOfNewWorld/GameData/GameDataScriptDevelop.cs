using System.Reactive;
using System.Reactive.Linq;
using EcsRx.Extensions;
using Game.Engine;
using Game.Engine.Core;
using Game.Engine.Core.Movement;
using Game.Engine.Core.Rooms;
using Game.Engine.Core.Rooms.Types;
using Game.Engine.Packageing.ScriptHosting.Scripts;
using Game.Engine.Screens;
using Terminal.Gui;

namespace RaiseOfNewWorld.GameData;

public class GameDataScriptDevelop : GlobalScriptVariables
{
    public GameDataScriptDevelop()
        : base(null!, null!, null!)
    {
    }

    public Unit Run()
    {
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
    }
}