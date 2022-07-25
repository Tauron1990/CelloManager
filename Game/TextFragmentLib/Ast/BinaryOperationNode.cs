namespace TextFragmentLib.Ast;

public sealed class BinaryOperationNode : ExpressionOperationNode
{
    public BinaryOperationNode(BinaryOperationType operationType, ExpressionNode left, ExpressionNode right)
        : base(operationType)
    {
        Left = left;
        Right = right;
    }


    public ExpressionNode Left { get; }

    public ExpressionNode Right { get; }


    protected override string Format()
        => $"{Left}{FormatOperationType()}{Right}";
}