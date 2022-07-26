﻿using System.Linq.Expressions;
using System.Text;
using Microsoft.Extensions.ObjectPool;

namespace RaiseOfNewWorld.Engine.Data;

public static class Pools
{
    public static readonly ObjectPool<StringBuilder> StringBuildersPool =
        ObjectPool.Create(new StringBuilderPooledObjectPolicy());

    public static readonly ObjectPool<List<TextFragment>> FragmentPool =
        ObjectPool.Create(new ListPoolPolicy<TextFragment>());

    public static readonly ObjectPool<List<Expression>> ExpressionPool =
        ObjectPool.Create(new ListPoolPolicy<Expression>());

    private sealed class ListPoolPolicy<TType> : PooledObjectPolicy<List<TType>>
    {
        public override List<TType> Create() => new();

        public override bool Return(List<TType> obj)
        {
            if (obj.Capacity < 500) return false;
            obj.Clear();
            return true;
        }
    }
}