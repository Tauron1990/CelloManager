using EcsRx.Components;
using EcsRx.ReactiveData;

namespace RaiseOfNewWorld.Engine.Player;

public sealed record PlayerComponent
    (ReactiveProperty<string> PlayerName, ReactiveProperty<string> DisplayName) : IComponent;