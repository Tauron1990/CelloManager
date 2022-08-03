using EcsRx.Components;
using EcsRx.ReactiveData;

namespace Game.Engine.Core.Movement;

public sealed record MoveableComponent(string Id, ReactiveProperty<string> Position) : IComponent;