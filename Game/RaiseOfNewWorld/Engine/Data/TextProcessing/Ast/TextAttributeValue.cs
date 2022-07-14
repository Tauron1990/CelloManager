namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

public sealed class TextAttributeValue : AttributeValueNode
{
    public string Value { get; set; } = string.Empty;

    public bool IsReference { get; set; }
    
    protected override string Format()
        => IsReference ? $"@{Value}" : Value;
}