namespace TextFragmentLib.Ast;

public sealed class UnaryOperationNode : ExpressionOperationNode
{
    public ExpressionNode Operant { get; }
    
    public UnaryOperationNode(BinaryOperationType operationType, ExpressionNode operant) : base(operationType)
        => Operant = operant;

    protected override string Format()
        => $"{FormatOperationType()}{Operant}";
}