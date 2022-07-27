using TextFragmentLib2.TextProcessing.Ast;
using TextFragmentLib2.TextProcessing.Parsing;

namespace TextFragmentLib2.TextProcessing.PrimitiveVisitor;

public sealed class BoolVisitor : ExpressionNodeVisitor<bool>
{
    public static readonly BoolVisitor Instance = new();

    public override bool VisitCall(CallExpressionNode callExpressionNode)
        => callExpressionNode.MethodName switch
        {
            "not" => callExpressionNode.Parameters.Select(Accept).All(b => !b),
            _ => callExpressionNode.Parameters.Select(Accept).All(b => b)
        };

    public override bool VisitExpression(BinaryExpressionNode binaryExpressionNode)
    {
        var valueLeft = Accept(binaryExpressionNode.Left);
        var valueRight = Accept(binaryExpressionNode.Right);

        return binaryExpressionNode.OperatorType switch
        {
            OperatorType.Add => valueLeft && valueRight,
            OperatorType.Subtract => valueLeft || valueRight,
            _ => throw new InvalidOperationException("Invalid operation type for Boolean")
        };
    }

    public override bool VisitText(LiteralExpressionNode literalExpressionNode)
        => throw new NotImplementedException();

    public static bool Evaluate(ExpressionBaseNode value)
        => Instance.Accept(value);
}