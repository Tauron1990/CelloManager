using System;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace CelloManager.Avalonia.ViewModels;

public sealed class SerialDisposableSubject<TData> : IDisposable
{
    private readonly SerialDisposable _disposer = new();
    private readonly BehaviorSubject<TData> _dataHolder;

    public SerialDisposableSubject(TData initial) => _dataHolder = new BehaviorSubject<TData>(initial);

    
    
    public void Dispose()
    {
        _disposer.Dispose();
        _dataHolder.Dispose();
    }
}