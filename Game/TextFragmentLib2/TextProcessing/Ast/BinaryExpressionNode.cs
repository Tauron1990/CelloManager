using System.Text;
using TextFragmentLib2.TextProcessing.Parsing;

namespace TextFragmentLib2.TextProcessing.Ast;

public enum OperatorType
{
    None,
    Subtract,
    Add,
    And,
    Or,
    Mult,
    Div,
    Equal,
    NotEqual,
    Not
}

public sealed class BinaryExpressionNode : ExpressionBaseNode
{
    public OperatorType OperatorType { get; }

    public ExpressionBaseNode Left { get; }

    public ExpressionBaseNode Right { get; }

    public BinaryExpressionNode(OperatorType operatorType, ExpressionBaseNode left, ExpressionBaseNode right)
    {
        OperatorType = operatorType;
        Left = left;
        Right = right;
    }

    public override void Validate()
    {
        if (OperatorType == OperatorType.None)
            ThrowValidationError("No operator type provided");
        if (Left == Empty)
            ThrowValidationError("No Left Operant for Expression");
        if (Right == Empty)
            ThrowValidationError("No Right Operant for Expression");

        Left.Validate();
        Right.Validate();
    }

    protected override string Format()
    {
        var builder = new StringBuilder();

        builder.Append(Left);

        builder.Append(FormatOperatorType(OperatorType));

        builder.Append(Right);

        return builder.ToString();
    }

    public override TReturn Visit<TReturn>(ExpressionNodeVisitor<TReturn> visitor)
        => visitor.VisitExpression(this);
}