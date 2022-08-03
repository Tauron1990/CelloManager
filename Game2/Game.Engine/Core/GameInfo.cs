using EcsRx.Components;
using EcsRx.ReactiveData;

namespace Game.Engine.Core;

public sealed record GameInfo(ReactiveProperty<bool> IsNewGame, ReactiveProperty<int> LastDimension) : IComponent;