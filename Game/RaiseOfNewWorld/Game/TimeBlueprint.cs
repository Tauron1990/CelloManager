using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Extensions;
using RaiseOfNewWorld.Engine;
using RaiseOfNewWorld.Engine.Data;
using RaiseOfNewWorld.Engine.Time;

namespace RaiseOfNewWorld.Game;

public sealed class TimeBlueprint : IBlueprint
{
    private readonly ContentManager _contentManager;

    public TimeBlueprint(ContentManager contentManager) => _contentManager = contentManager;

    public void Apply(IEntity entity)
    {
        var broken = new TimeLine(_contentManager.GetString("BrokenTimeLineName"), _contentManager.GetInt("BrokenTimeLineShift"));
        var main =new TimeLine(_contentManager.GetString("MainTimeLineName"), _contentManager.GetInt("MainTimeLineTimeShift"));
        
        entity.AddComponent(
            new GlobalBaseTimeComponent(
                RxProperty.New(_contentManager.GetDateTime("GlobalBaseTime")),
                RxProperty.New(broken)));

        entity.AddComponent(new CurrentTimeLine(broken, main));
    }
}