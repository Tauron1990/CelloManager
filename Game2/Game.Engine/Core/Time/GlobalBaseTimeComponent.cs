using EcsRx.Components;
using EcsRx.ReactiveData;

namespace Game.Engine.Core.Time;

public sealed record GlobalBaseTimeComponent(ReactiveProperty<DateTime> CurrentBaseTime,
    ReactiveProperty<TimeLine> CurrentTimeline) : IComponent;