using EcsRx.Collections;
using SystemsRx.Extensions;
using SystemsRx.Systems.Conventional;

namespace RaiseOfNewWorld.Engine.Time;

public class TimeManager : IReactToEventSystem<IConsumesTime>
{
    private readonly ComponentList<GlobalBaseTimeComponent> _timeEntitys;
    
    public TimeManager(IObservableGroupManager groupManager) => _timeEntitys = new ComponentList<GlobalBaseTimeComponent>(groupManager, 0);

    public void Process(IConsumesTime eventData)
    {
        if(eventData.TimeNeed == TimeSpan.Zero) return;
        
        _timeEntitys.ForEachRun(c => c.CurrentBaseTime.Value += eventData.TimeNeed);
    }
}