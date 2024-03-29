﻿using EcsRx.Collections;
using Game.Engine.Core.Movement;
using SystemsRx.Extensions;
using SystemsRx.Systems.Conventional;

namespace Game.Engine.Core;

public sealed class GameInfoManager : IReactToEventSystem<SwitchDimesionEvent>
{
    private readonly ComponentList<GameInfo> _gameInfos;

    public GameInfoManager(IObservableGroupManager observableGroupManager)
    {
        _gameInfos = new ComponentList<GameInfo>(
            observableGroupManager,
            0);
    }

    public void Process(SwitchDimesionEvent eventData)
        => _gameInfos.ForEachRun(i => i.LastDimension.Value = eventData.Dimesion);
}