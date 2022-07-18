using System.Text;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

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