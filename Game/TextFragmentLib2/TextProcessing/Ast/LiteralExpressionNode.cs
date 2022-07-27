using TextFragmentLib2.TextProcessing.Parsing;

namespace TextFragmentLib2.TextProcessing.Ast;

public sealed class LiteralExpressionNode : ExpressionBaseNode
{
    public string Value { get; }

    public bool IsReference { get; }

    public LiteralExpressionNode(string value, bool isReference)
    {
        Value = value;
        IsReference = isReference;
    }
    
    public override void Validate()
    {
        if (IsReference && string.IsNullOrWhiteSpace(Value))
            ThrowValidationError("For Refernce an Path is Requiered");
    }

    protected override string Format()
        => IsReference ? $"@{Value}" : Value;

    public override ExpressionBaseNode Merge(ExpressionBaseNode node)
    {
        if (node is LiteralExpressionNode txt)
            return txt;
        return base.Merge(node);
    }

    public override TReturn Visit<TReturn>(ExpressionNodeVisitor<TReturn> visitor)
        => visitor.VisitText(this);
}