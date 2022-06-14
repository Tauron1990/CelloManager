using EcsRx.Components;
using EcsRx.ReactiveData;

namespace RaiseOfNewWorld.Engine.Movement;

public sealed record MoveableComponent(string Id, ReactiveProperty<string> Position) : IComponent;