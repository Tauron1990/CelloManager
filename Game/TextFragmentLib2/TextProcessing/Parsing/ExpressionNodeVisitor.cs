using TextFragmentLib2.TextProcessing.Ast;

namespace TextFragmentLib2.TextProcessing.Parsing;

public abstract class ExpressionNodeVisitor<TReturn>
{
    public TReturn Accept(ExpressionBaseNode node)
        => node.Visit(this);

    public abstract TReturn VisitCall(CallExpressionNode callExpressionNode);

    public abstract TReturn VisitExpression(BinaryExpressionNode binaryExpressionNode);

    public abstract TReturn VisitText(LiteralExpressionNode literalExpressionNode);

    protected static string ResolveTextAttribute(LiteralExpressionNode value)
        => Pools.ResolveResource(value);

    public abstract TReturn VisitUnary(UnaryExpressionNode unaryExpressionNode);
}