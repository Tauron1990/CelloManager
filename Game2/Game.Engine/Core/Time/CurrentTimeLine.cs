using EcsRx.Components;

namespace Game.Engine.Core.Time;

public sealed record CurrentTimeLine
(
    TimeLine BrokenTimeLine,
    TimeLine MainTimeLine
) : IComponent;