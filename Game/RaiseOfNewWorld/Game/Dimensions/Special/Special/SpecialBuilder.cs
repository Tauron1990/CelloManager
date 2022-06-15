using EcsRx.Extensions;
using RaiseOfNewWorld.Engine.Data;
using RaiseOfNewWorld.Engine.Movement;
using RaiseOfNewWorld.Engine.Rooms.Maps;
using RaiseOfNewWorld.Engine.Rooms.Types;
using RaiseOfNewWorld.Screens.GameScreens;

namespace RaiseOfNewWorld.Game.Dimensions.Special.Special;

public static class SpecialBuilder
{
    public static void InitSpecial(DimensionBuilder builder)
    {
        builder.WithDimension(1, b =>
        {
            b.WithRoom("start", r =>
            {
                r.WithFactory((_, manager) => new TextDisplay(TextParser.Pages(manager.GetStringFunc("PrologText")),
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

            b.WithRoom("end", rb =>
            {
                rb.WithFactory((_, manager) => new TextDisplay(
                    TextParser.Pages(manager.GetStringFunc("EpilogText")),
                    static m => m.ScreenManager.Switch(nameof(MainScreen), new Action(m.ClearGame))));
            });
        });
    }
}