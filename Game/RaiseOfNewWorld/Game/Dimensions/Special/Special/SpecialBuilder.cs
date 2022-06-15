using EcsRx.Extensions;
using RaiseOfNewWorld.Engine.Data;
using RaiseOfNewWorld.Engine.Rooms.Maps;
using RaiseOfNewWorld.Engine.Rooms.Types;

namespace RaiseOfNewWorld.Game.Dimensions.Special.Special;

public static class SpecialBuilder
{
    public static void InitSpecial(DimensionBuilder builder)
    {
        builder.WithDimension(1, b =>
        {
            b.WithRoom("start", r =>
            {
                r.WithFactory((_, manager) => new TextDisplay(TextParser.ParsePages(manager.GetString("PrologText")), m =>
                {
                    var gameInfo = m.Database.GetCollection()
                        .Where(e => e.HasComponent(typeof(GameInfo)))
                        .Select(e => e.GetComponent<GameInfo>())
                        .First();
                    gameInfo.IsNewGame.Value = false;
                    
                    m.ShutdownApp();
                }));
            });
        });
    }
}