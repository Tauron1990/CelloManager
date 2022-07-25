using TextFragmentLib2.TextProcessing.Ast;

namespace TextFragmentLib2.TextProcessing.ParsingOld;

public abstract class AttributeValueVisitor<TReturn>
{
    public TReturn Accept(AttributeValueNode node)
        => node.Visit(this);

    public abstract TReturn VisitCall(CallAttributeValue callAttributeValue);

    public abstract TReturn VisitExpression(ExpressionAttributeValue expressionAttributeValue);

    public abstract TReturn VisitText(TextAttributeValue textAttributeValue);

    protected static string ResolveTextAttribute(TextAttributeValue value)
        => Pools.ResolveResource(value);
}