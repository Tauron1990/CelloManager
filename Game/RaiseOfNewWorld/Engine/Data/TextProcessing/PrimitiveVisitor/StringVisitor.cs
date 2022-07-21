using System.Text;
using RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;
using RaiseOfNewWorld.Engine.Data.TextProcessing.Parsing;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing.PrimitiveVisitor;

public sealed class StringVisitor : AttributeValueVisitor<StringBuilder>
{
    public static readonly StringVisitor Instance = new();

    public string ToString(AttributeValueNode node)
    {
        var builder = Accept(node);
        try
        {
            return builder.ToString();
        }
        finally
        {
            Pools.StringBuildersPool.Return(builder);
        }
    }

    public override StringBuilder VisitCall(CallAttributeValue callAttributeValue)
    {
        var builder = Pools.StringBuildersPool.Get();

        foreach (var subBuilder in callAttributeValue.Parameters.Select(Accept))
            try
            {
                builder.Append(subBuilder);
            }
            finally
            {
                Pools.StringBuildersPool.Return(subBuilder);
            }

        return builder;
    }

    public override StringBuilder VisitExpression(ExpressionAttributeValue expressionAttributeValue)
    {
        var leftBuilder = Accept(expressionAttributeValue.Left);
        var rightBuilder = Accept(expressionAttributeValue.Right);
        try
        {
            var target = Pools.StringBuildersPool.Get();

            return expressionAttributeValue.OperatorType switch
            {
                OperatorType.Subtract => Subtract(
                    target,
                    leftBuilder,
                    rightBuilder),
                _ => target.Append(leftBuilder).Append(rightBuilder)
            };
        }
        finally
        {
            Pools.StringBuildersPool.Return(rightBuilder);
            Pools.StringBuildersPool.Return(leftBuilder);
        }

        StringBuilder Subtract(StringBuilder target, StringBuilder input, StringBuilder subtract)
        {
            if (subtract.Length == 0)
                return target.Append(input);

            var start = subtract[0];
            for (var i = 0; i < input.Length; i++)
            {
                if (input[i] != start) continue;
                var found = false;

                for (var j = 1; j < subtract.Length; j++)
                {
                    var inputIndex = i + j;
                    if (inputIndex < input.Length) break;

                    found = input[inputIndex] == subtract[j];
                    if (found) continue;
                    break;
                }

                if (found)
                    return target.Append(input).Remove(
                        i,
                        subtract.Length);
            }

            return target;
        }
    }

    public override StringBuilder VisitText(TextAttributeValue textAttributeValue)
        => Pools.StringBuildersPool.Get().Append(ResolveTextAttribute(textAttributeValue));
}