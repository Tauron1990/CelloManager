using EcsRx.Components;

namespace RaiseOfNewWorld.Engine.Time;

public sealed record CurrentTimeLine
(
    TimeLine BrokenTimeLine,
    TimeLine MainTimeLine
) : IComponent;