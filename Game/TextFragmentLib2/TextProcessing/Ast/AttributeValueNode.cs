using TextFragmentLib2.TextProcessing.ParsingOld;

namespace TextFragmentLib2.TextProcessing.Ast;

public abstract class AttributeValueNode : AstNode
{
    public static readonly AttributeValueNode Empty = new EmptyNode();

    public abstract TReturn Visit<TReturn>(AttributeValueVisitor<TReturn> visitor);

    public virtual AttributeValueNode Merge(AttributeValueNode node)
        => new ExpressionAttributeValue { Left = this, Right = node };

    protected sealed class EmptyNode : AttributeValueNode
    {
        public override void Validate()
        {
        }

        protected override string Format()
            => string.Empty;

        public override TReturn Visit<TReturn>(AttributeValueVisitor<TReturn> visitor)
            => throw new InvalidOperationException("An Empty Node sould not be Encounterd");

        public override AttributeValueNode Merge(AttributeValueNode node)
            => node;
    }
    
    protected string FormatOperatorType(OperatorType operatorType)
        => operatorType switch
        {
            OperatorType.Subtract => "-",
            OperatorType.Add => "+",
            OperatorType.None => "n",
            OperatorType.And => "and",
            OperatorType.Or => "or",
            OperatorType.Mult => "*",
            OperatorType.Div => ":",
            OperatorType.Equal => "==",
            OperatorType.NotEqual => "!=",
            OperatorType.Not => "!",
            _ => throw new ArgumentOutOfRangeException()
        };
}