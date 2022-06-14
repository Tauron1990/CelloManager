using RaiseOfNewWorld.Engine.Rooms.Maps;

namespace RaiseOfNewWorld.Game.Dimensions.Special.Special;

public static class SpecialBuilder
{
    public static void InitSpecial(DimensionBuilder builder)
    {
        builder.WithDimension(1, b =>
        {
            b.WithRoom("start", r =>
            {
                r.WithFactory((roomBuilder, manager) => manager.GetDateTime());
            });
        });
    }
}