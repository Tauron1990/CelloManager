namespace TextFragmentLib.Ast;

public sealed class BinaryOperationNode : ExpressionOperationNode
{


    public ExpressionNode Left { get; }

    public ExpressionNode Right { get; }

    public BinaryOperationNode(BinaryOperationType operationType, ExpressionNode left, ExpressionNode right)
        : base(operationType)
    {
        Left = left;
        Right = right;
    }



    protected override string Format()
        => $"{Left}{FormatOperationType()}{Right}";
}