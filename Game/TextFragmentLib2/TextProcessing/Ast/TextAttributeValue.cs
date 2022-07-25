using TextFragmentLib2.TextProcessing.ParsingOld;

namespace TextFragmentLib2.TextProcessing.Ast;

public sealed class TextAttributeValue : AttributeValueNode
{
    public string Value { get; set; } = string.Empty;

    public bool IsReference { get; set; }

    public override void Validate()
    {
        if (IsReference && string.IsNullOrWhiteSpace(Value))
            ThrowValidationError("For Refernce an Path is Requiered");
    }

    protected override string Format()
        => IsReference ? $"@{Value}" : Value;

    public override AttributeValueNode Merge(AttributeValueNode node)
    {
        if (node is TextAttributeValue txt)
            return txt;
        return base.Merge(node);
    }

    public override TReturn Visit<TReturn>(AttributeValueVisitor<TReturn> visitor)
        => visitor.VisitText(this);
}