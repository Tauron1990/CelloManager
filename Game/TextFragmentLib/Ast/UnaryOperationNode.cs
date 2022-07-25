namespace TextFragmentLib.Ast;

public sealed class UnaryOperationNode : ExpressionOperationNode
{
    public UnaryOperationNode(BinaryOperationType operationType, ExpressionNode operant) : base(operationType)
    {
        Operant = operant;
    }

    public ExpressionNode Operant { get; }

    protected override string Format()
        => $"{FormatOperationType()}{Operant}";
}