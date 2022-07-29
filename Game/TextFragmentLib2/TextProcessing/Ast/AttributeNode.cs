namespace TextFragmentLib2.TextProcessing.Ast;

public sealed class AttributeNode : AstNode
{
    public string Name { get; }

    public ExpressionBaseNode Value { get; set; }

    public AttributeNode(string name, ExpressionBaseNode value)
    {
        Name = name;
        Value = value;
    }

    public override void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            ThrowValidationError("Name is null of whitespace");
        if (Value == ExpressionBaseNode.Empty)
            ThrowValidationError($"No Attribute Value: {Name} ");

        Value.Validate();
    }

    protected override string Format()
        => $"{Name}:{Value}";
}