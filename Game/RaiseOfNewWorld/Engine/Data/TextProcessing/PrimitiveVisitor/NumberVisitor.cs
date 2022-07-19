using RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing.PrimitiveVisitor;

public abstract class NumberVisitor<TNumber> : AttributeValueVisitor<TNumber>
{
    public override TNumber VisitCall(CallAttributeValue callAttributeValue)
    {
        throw new NotImplementedException();
    }

    public override TNumber VisitExpression(ExpressionAttributeValue expressionAttributeValue)
    {
        throw new NotImplementedException();
    }

    public override TNumber VisitText(TextAttributeValue textAttributeValue)
    {
        throw new NotImplementedException();
    }

    protected abstract TNumber Add(TNumber numberLeft, TNumber numberRight);
    
    protected abstract TNumber Substract(TNumber numberLeft, TNumber numberRight);
    
    protected abstract TNumber Devide(TNumber numberLeft, TNumber numberRight);
    
    protected abstract TNumber Multiply(TNumber numberLeft, TNumber numberRight);
}