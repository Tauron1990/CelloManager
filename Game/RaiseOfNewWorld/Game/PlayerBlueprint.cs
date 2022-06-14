using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Extensions;
using RaiseOfNewWorld.Engine;
using RaiseOfNewWorld.Engine.Movement;
using RaiseOfNewWorld.Engine.Player;

namespace RaiseOfNewWorld.Game;

public sealed class PlayerBlueprint : IBlueprint
{
    public void Apply(IEntity entity)
    {
        entity.AddComponent(new PlayerComponent(RxProperty.New(string.Empty), RxProperty.New(string.Empty)));
        entity.AddComponent(new MoveableComponent("player", RxProperty.New(string.Empty)));
    }
}