using System.Text;
using TextFragmentLib2.TextProcessing.ParsingOld;

namespace TextFragmentLib2.TextProcessing.Ast;

public enum OperatorType
{
    None,
    Subtract,
    Add
}

public sealed class ExpressionAttributeValue : AttributeValueNode
{
    public OperatorType OperatorType { get; set; } = OperatorType.None;

    public AttributeValueNode Left { get; set; } = Empty;

    public AttributeValueNode Right { get; set; } = Empty;


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
        switch (OperatorType)
        {
            case OperatorType.None:
                builder.Append('n');
                break;
            case OperatorType.Subtract:
                builder.Append('-');
                break;
            case OperatorType.Add:
                builder.Append('+');
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        builder.Append(Right);

        return builder.ToString();
    }

    public override TReturn Visit<TReturn>(AttributeValueVisitor<TReturn> visitor)
        => visitor.VisitExpression(this);
}