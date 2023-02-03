using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using DynamicData;

namespace CelloManager.Core.DataOperators;

internal sealed class TransformUpdate<TDestination, TSource, TKey>
    where TKey : notnull
    where TDestination : IUpdateAware<TSource>
{
    private readonly IObservable<IChangeSet<TSource, TKey>> _source;
    private readonly Func<TSource, TDestination> _transformFactory;
    private readonly Dictionary<TKey, TDestination> _elemnts = new();

    public TransformUpdate(
        IObservable<IChangeSet<TSource, TKey>> source,
        Func<TSource, TDestination> transformFactory)
    {
        _source = source;
        _transformFactory = transformFactory;
    }

    public IObservable<IChangeSet<TDestination, TKey>> Run()
    {
        return _source

            .Scan(
                (ChangeAwareCache<TDestination, TKey>?)null,
                (cache, changes) =>
                {
                    cache ??= new ChangeAwareCache<TDestination, TKey>(changes.Count);

                    foreach (var change in changes)
                    {
                        switch (change.Reason)
                        {
                            case ChangeReason.Add:
                                var element = _transformFactory(change.Current);
                                _elemnts.Add(change.Key, element);
                                cache.AddOrUpdate(element, change.Key);
                                break;
                            case ChangeReason.Remove:
                                _elemnts.Remove(change.Key);
                                cache.Remove(change.Key);
                                break;
                            case ChangeReason.Update:
                            case ChangeReason.Refresh:
                                _elemnts[change.Key].Update(change.Current);
                                cache.Refresh(change.Key);
                                break;
                        }
                    }

                    return cache;
                })
            .Where(x => x is not null)
            .Select(cache => cache!.CaptureChanges())
            .NotEmpty();
    }
}