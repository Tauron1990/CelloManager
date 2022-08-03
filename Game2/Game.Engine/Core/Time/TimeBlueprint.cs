using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Extensions;
using Game.Engine.Packageing.Files;

namespace Game.Engine.Core.Time;

public sealed class TimeBlueprint : IBlueprint
{
    private readonly GameContentManager _contentManager;

    public TimeBlueprint(GameContentManager contentManager)
    {
        _contentManager = contentManager;
    }

    public void Apply(IEntity entity)
    {
        var broken = new TimeLine(
            _contentManager.GetString("BrokenTimeLineName"),
            _contentManager.GetInt("BrokenTimeLineShift"));
        var main = new TimeLine(
            _contentManager.GetString("MainTimeLineName"),
            _contentManager.GetInt("MainTimeLineTimeShift"));

        entity.AddComponent(
            new GlobalBaseTimeComponent(
                RxProperty.New(_contentManager.GetDateTime("GlobalBaseTime")),
                RxProperty.New(broken)));

        entity.AddComponent(
            new CurrentTimeLine(
                broken,
                main));
    }
}