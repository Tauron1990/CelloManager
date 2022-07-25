using System.Linq.Expressions;
using System.Text;
using Microsoft.Extensions.ObjectPool;
using TextFragmentLib2.TextProcessing.Ast;

namespace TextFragmentLib2;

public static class Pools
{
    public static readonly ObjectPool<StringBuilder> StringBuildersPool =
        ObjectPool.Create(new StringBuilderPooledObjectPolicy());

    public static readonly ObjectPool<List<Expression>> ExpressionPool =
        ObjectPool.Create(new ListPoolPolicy<Expression>());

    public static Func<TextAttributeValue, string> ResolveResource { get; set; } = s => s.Value;

    public static Func<string, string> ReadFile { get; set; } = s => s;

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