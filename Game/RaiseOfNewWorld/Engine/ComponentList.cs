using System.Reactive.Linq;
using EcsRx.Collections;
using EcsRx.Components;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Plugins.Computeds.Collections;

namespace RaiseOfNewWorld.Engine;

public sealed class ComponentList<TComponent> : ComputedCollectionFromGroup<TComponent>
    where TComponent : class, IComponent
{
    public ComponentList(IObservableGroupManager groupManager, params int[] ids)
        : base(
            groupManager.GetObservableGroup(
                new Group(typeof(TComponent)),
                ids))
    {
    }

    public override IObservable<bool> RefreshWhen()
        => InternalObservableGroup.OnEntityAdded.Merge(InternalObservableGroup.OnEntityRemoved).Select(_ => true);

    public override bool ShouldTransform(IEntity entity)
        => true;

    public override TComponent Transform(IEntity entity)
        => entity.GetComponent<TComponent>();
}