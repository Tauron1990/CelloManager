using EcsRx.Components;
using EcsRx.ReactiveData;

namespace RaiseOfNewWorld.Engine.Time;

public sealed record GlobalBaseTimeComponent(ReactiveProperty<DateTime> CurrentBaseTime,
    ReactiveProperty<TimeLine> CurrentTimeline) : IComponent;