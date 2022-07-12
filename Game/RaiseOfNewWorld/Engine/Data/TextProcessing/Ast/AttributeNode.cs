namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

public sealed class AttributeNode : AstNode
{
    public string Name { get; set; } = string.Empty;

    public AttributeValueNode Value { get; set; } = AttributeValueNode.Empty;

    protected override string Format()
        => $"{Name}:{Value}";
}