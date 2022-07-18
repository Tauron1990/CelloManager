namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

public sealed class TextAttributeValue : AttributeValueNode
{
    public string Value { get; set; } = string.Empty;

    public bool IsReference { get; set; }
    
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