namespace TextFragmentLib.Ast;

public sealed class ExpressionLiteralNode : ExpressionNode
{
    public string Text { get; }

    public ExpressionLiteralNode(string text)
    {
        Text = text;
    }

    public override void Validate()
    {
        if(string.IsNullOrWhiteSpace(Text))
            ThrowValidationError("Literal text is empty");
    }

    protected override string Format()
        => Text;
}