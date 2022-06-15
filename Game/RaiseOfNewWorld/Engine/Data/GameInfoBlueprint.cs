using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Extensions;

namespace RaiseOfNewWorld.Engine.Data;

public sealed class GameInfoBlueprint : IBlueprint
{
    public void Apply(IEntity entity)
    {
        entity.AddComponent(new GameInfo(RxProperty.New(true), RxProperty.New(-1)));
    }
}