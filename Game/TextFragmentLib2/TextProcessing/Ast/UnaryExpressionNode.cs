using TextFragmentLib2.TextProcessing.Parsing;

namespace TextFragmentLib2.TextProcessing.Ast;

public sealed class UnaryExpressionNode : ExpressionBaseNode
{
    public OperatorType Type { get; }

    public ExpressionBaseNode Expression { get; }

    public UnaryExpressionNode(OperatorType type, ExpressionBaseNode expressions)
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

    public override TReturn Visit<TReturn>(ExpressionNodeVisitor<TReturn> visitor)
        => visitor.VisitUnary(this);
}