﻿using System;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace CelloManager.ViewModels;

public sealed class SerialDisposableSubject<TData> : IDisposable, IObservable<TData>, IObserver<TData>
{
    private readonly SerialDisposable _disposer = new();
    private readonly BehaviorSubject<TData> _dataHolder;

    public SerialDisposableSubject(TData initial)
    {
        _disposer.Disposable = initial as IDisposable;
        _dataHolder = new BehaviorSubject<TData>(initial);
    }


    public void Dispose()
    {
        _disposer.Dispose();
        _dataHolder.Dispose();
    }

    IDisposable IObservable<TData>.Subscribe(IObserver<TData> observer) => _dataHolder.Subscribe(observer);

    void IObserver<TData>.OnCompleted() => _dataHolder.OnCompleted();

    void IObserver<TData>.OnError(Exception error) => _dataHolder.OnError(error);

    public void OnNext(TData value)
    {
        _dataHolder.OnNext(value);
        _disposer.Disposable = value as IDisposable;
    }
}