using EcsRx.ReactiveData;

namespace Game.Engine;

public static class RxProperty
{
    public static ReactiveProperty<TType> New<TType>(TType value) => new(value);
}