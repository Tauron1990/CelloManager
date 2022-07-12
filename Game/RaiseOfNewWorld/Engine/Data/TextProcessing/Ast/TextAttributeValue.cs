namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

public sealed class TextAttributeValue : AttributeValueNode
{
    public string Value { get; set; } = string.Empty;

    protected override string Format()
        => Value;
}