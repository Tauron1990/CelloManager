using RaiseOfNewWorld.Engine.Data.TextProcessing.Parsing;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

public abstract class AttributeValueNode : AstNode
{
    public static readonly AttributeValueNode Empty = new EmptyNode();

    protected sealed class EmptyNode : AttributeValueNode
    {
        public override void Validate() { }

        protected override string Format()
            => string.Empty;

        public override TReturn Visit<TReturn>(AttributeValueVisitor<TReturn> visitor)
            => throw new InvalidOperationException("An Empty Node sould not be Encounterd");

        public override AttributeValueNode Merge(AttributeValueNode node)
            => node;
    }

    public abstract TReturn Visit<TReturn>(AttributeValueVisitor<TReturn> visitor);

    public virtual AttributeValueNode Merge(AttributeValueNode node)
        => new ExpressionAttributeValue { Left = this, Right = node };
}