using System;
using DynamicData;

namespace CelloManager.Core.DataOperators;

public static class Exstensions
{
    public static IObservable<IChangeSet<TDestination, TKey>> SelectUpdate<TDestination, TSource, TKey>(
        this IObservable<IChangeSet<TSource, TKey>> input, Func<TSource, TDestination> transform) 
        where TDestination : IUpdateAware<TSource> where TKey : notnull =>
        new TransformUpdate<TDestination, TSource, TKey>(input, transform).Run();
}