using EcsRx.Components;
using EcsRx.ReactiveData;

namespace Game.Engine.Core.Player;

public sealed record PlayerComponent
    (ReactiveProperty<string> PlayerName, ReactiveProperty<string> DisplayName) : IComponent;