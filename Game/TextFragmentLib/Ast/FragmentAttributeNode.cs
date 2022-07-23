namespace TextFragmentLib.Ast;

public sealed class FragmentAttributeNode : FragmentNode
{
    public string Name { get; }

    public ExpressionNode Node { get; }

    public FragmentAttributeNode(string name, ExpressionNode node)
    {
        Name = name;
        Node = node;
    }

    public override void Validate()
    {
        if(string.IsNullOrEmpty(Name))
            ThrowValidationError("No Name for Attribute");
    }

    protected override string Format()
        => throw new NotImplementedException();
}