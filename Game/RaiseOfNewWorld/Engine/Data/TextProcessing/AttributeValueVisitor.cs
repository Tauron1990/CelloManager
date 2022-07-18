using RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing;

public abstract class AttributeValueVisitor<TReturn>
{
    public TReturn Accept(AttributeValueNode node) 
        => node.Visit(this);

    public abstract TReturn VisitCall(CallAttributeValue callAttributeValue);

    public abstract TReturn VisitExpression(ExpressionAttributeValue expressionAttributeValue);

    public abstract TReturn VisitText(TextAttributeValue textAttributeValue);
}