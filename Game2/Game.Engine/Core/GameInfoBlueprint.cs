using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Extensions;

namespace Game.Engine.Core;

public sealed class GameInfoBlueprint : IBlueprint
{
    public void Apply(IEntity entity)
    {
        entity.AddComponent(
            new GameInfo(
                RxProperty.New(true),
                RxProperty.New(-1)));
    }
}