namespace TextFragmentLib.Ast;

public sealed class FragmentAttributeNode : FragmentNode
{
    public FragmentAttributeNode(string name, ExpressionNode node)
    {
        Name = name;
        Node = node;
    }

    public string Name { get; }

    public ExpressionNode Node { get; }

    public override void Validate()
    {
        if (string.IsNullOrEmpty(Name))
            ThrowValidationError("No Name for Attribute");
    }

    protected override string Format()
        => throw new NotImplementedException();
}