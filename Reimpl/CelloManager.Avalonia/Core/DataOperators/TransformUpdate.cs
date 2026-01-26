using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using DynamicData;

namespace CelloManager.Core.DataOperators;

internal sealed class TransformUpdate<TDestination, TSource, TKey>(
    IObservable<IChangeSet<TSource, TKey>> source,
    Func<TSource, TDestination> transformFactory)
    where TKey : notnull
    where TDestination : IUpdateAware<TSource>
    where TSource : notnull
{
    private readonly Dictionary<TKey, TDestination> _elemnts = new();

    public IObservable<IChangeSet<TDestination, TKey>> Run()
    {
        return source
              .Scan(
                   (ChangeAwareCache<TDestination, TKey>?)null,
                   (cache, changes) =>
                   {
                       cache ??= new ChangeAwareCache<TDestination, TKey>(changes.Count);

                       foreach (Change<TSource, TKey> change in changes)
                       {
                           switch (change.Reason)
                           {
                               case ChangeReason.Add:
                                   TDestination element = transformFactory(change.Current);
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
                               case ChangeReason.Moved:
                               default:
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