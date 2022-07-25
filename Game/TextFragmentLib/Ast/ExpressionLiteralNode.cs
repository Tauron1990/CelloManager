namespace TextFragmentLib.Ast;

public sealed class ExpressionLiteralNode : ExpressionNode
{
    public ExpressionLiteralNode(string text)
    {
        Text = text;
    }

    public string Text { get; }

    public override void Validate()
    {
        if (string.IsNullOrWhiteSpace(Text))
            ThrowValidationError("Literal text is empty");
    }

    protected override string Format()
        => Text;
}