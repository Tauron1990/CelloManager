using System.Text;
using Microsoft.Extensions.ObjectPool;

namespace RaiseOfNewWorld.Engine.Data;

public static class Pools
{
    private sealed class ListPoolPolicy<TType> : PooledObjectPolicy<List<TType>>
    {
        public override List<TType> Create() => new();

        public override bool Return(List<TType> obj)
        {
            obj.Clear();
            return true;
        }
    }
    
    public static readonly ObjectPool<StringBuilder> StringBuilders = ObjectPool.Create(new StringBuilderPooledObjectPolicy());

    public static readonly ObjectPool<List<TextFragment>> FragmentPools = ObjectPool.Create(new ListPoolPolicy<TextFragment>());

}