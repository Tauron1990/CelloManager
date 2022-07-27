namespace TextFragmentLib2.TextProcessing.Ast;

public sealed class AttributeNode : AstNode
{
    public string Name { get; set; } = string.Empty;

    public ExpressionBaseNode Value { get; set; } = ExpressionBaseNode.Empty;

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