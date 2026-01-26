using System;
using System.Collections.Generic;

namespace CelloManager;

public static class Extensions
{
    public static TValue DisposeWith<TValue>(this TValue disposable, ICollection<IDisposable> compositeDisposable)
    where TValue : IDisposable
    {
        compositeDisposable.Add(disposable);
        return disposable;
    }
}