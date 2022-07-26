using TextFragmentLib2.TextProcessing.ParsingOld;

namespace TextFragmentLib2.TextProcessing.Ast;

public sealed class UnaryAttributeValue : AttributeValueNode
{
    public OperatorType Type { get; }

    public AttributeValueNode Expression { get; }

    public UnaryAttributeValue(OperatorType type, AttributeValueNode expressions)
    {
        Type = type;
        Expression = expressions;
    }

    public override void Validate()
    {
        if (Type == OperatorType.None)
            ThrowValidationError("No Operator type Selected");
        
        if(Expression == Empty)
            ThrowValidationError("Empty Expression");
    }

    protected override string Format()
        => $"{FormatOperatorType(Type)} {Expression}";

    public override TReturn Visit<TReturn>(AttributeValueVisitor<TReturn> visitor)
        => visitor.VisitUnary(this);
}