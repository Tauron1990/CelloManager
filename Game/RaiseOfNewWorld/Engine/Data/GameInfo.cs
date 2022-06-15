using EcsRx.Components;
using EcsRx.ReactiveData;

namespace RaiseOfNewWorld.Engine.Data;

public sealed record GameInfo(ReactiveProperty<bool> IsNewGame, ReactiveProperty<int> LastDimension) : IComponent;