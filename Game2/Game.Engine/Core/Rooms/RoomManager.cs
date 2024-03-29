﻿using EcsRx.Collections;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using Game.Engine.Core.Movement;
using Game.Engine.Core.Rooms.Maps;
using SystemsRx.Events;
using SystemsRx.Systems.Conventional;

namespace Game.Engine.Core.Rooms;

public class RoomManager : IReactToEventSystem<MoveToRoom>, IReactToEventSystem<SwitchDimesionEvent>
{
    private readonly IEventSystem _eventSystem;
    private readonly IObservableGroupManager _groupManager;
    private readonly DimensionMap _map = DimensionMapBuilder.CreateMap();
    private readonly IGroup _movableGroup = new Group(typeof(MoveableComponent));

    private readonly IObservableGroup _movables;
    private readonly IGroup _roomGroup = new Group(typeof(RoomComponent));

    private IObservableGroup? _currentMap;

    public RoomManager(IObservableGroupManager groupManager, IEventSystem eventSystem)
    {
        _groupManager = groupManager;
        _eventSystem = eventSystem;
        _movables = groupManager.GetObservableGroup(
            _movableGroup,
            0);
    }

    public void Process(MoveToRoom eventData)
    {
        if (_currentMap is null) return;

        var roomComponent = _currentMap.Select(e => e.GetComponent<RoomComponent>())
            .First(c => c.Name == eventData.RoomName);
        var movableEntity = _movables.Select(e => e.GetComponent<MoveableComponent>()).First(c => c.Id == eventData.Id);
        movableEntity.Position.Value = roomComponent.Name;

        if (movableEntity.Id != "player") return;

        var roomBase = _map.GetMap(roomComponent.Dimesion).LookUp(roomComponent.Name);
        _eventSystem.Publish(new RenderRoom(roomBase));
    }

    public void Process(SwitchDimesionEvent eventData) => _currentMap = _groupManager.GetObservableGroup(
        _roomGroup,
        eventData.Dimesion);
}