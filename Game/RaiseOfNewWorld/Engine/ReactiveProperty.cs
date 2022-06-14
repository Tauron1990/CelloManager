using EcsRx.ReactiveData;

namespace RaiseOfNewWorld.Engine;

public static class RxProperty
{
    public static ReactiveProperty<TType> New<TType>(TType value) => new(value);
}