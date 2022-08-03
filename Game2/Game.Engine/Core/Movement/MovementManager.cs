using EcsRx.Collections;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using SystemsRx.Extensions;
using SystemsRx.Systems.Conventional;

namespace Game.Engine.Core.Movement;

public sealed class MovementManager : IReactToEventSystem<MoveToRoom>, IReactToEventSystem<SwitchDimesionEvent>
{
    private static readonly IGroup GroupFilter = new Group(typeof(MoveableComponent));
    private readonly IObservableGroupManager _manager;
    private IObservableGroup _observableGroup;

    public MovementManager(IObservableGroupManager manager)
    {
        _manager = manager;
        _observableGroup = manager.GetObservableGroup(
            GroupFilter,
            0);
    }

    public void Process(MoveToRoom eventData)
        => _observableGroup
            .Select(e => e.GetComponent<MoveableComponent>())
            .Where(mc => mc.Id == eventData.Id)
            .ForEachRun(e => e.Position.Value = eventData.RoomName);

    public void Process(SwitchDimesionEvent eventData) => _observableGroup = _manager.GetObservableGroup(
        GroupFilter,
        0,
        eventData.Dimesion);
}