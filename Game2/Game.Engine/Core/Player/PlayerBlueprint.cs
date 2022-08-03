using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Extensions;
using Game.Engine.Core.Movement;

namespace Game.Engine.Core.Player;

public sealed class PlayerBlueprint : IBlueprint
{
    public void Apply(IEntity entity)
    {
        entity.AddComponent(
            new PlayerComponent(
                RxProperty.New(string.Empty),
                RxProperty.New(string.Empty)));
        entity.AddComponent(
            new MoveableComponent(
                "player",
                RxProperty.New(string.Empty)));
    }
}