namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

public abstract class AttributeValueNode : AstNode
{
    public static readonly AttributeValueNode Empty = new EmptyNode();

    protected sealed class EmptyNode : AttributeValueNode
    {
        protected override string Format()
            => string.Empty;
    }
}