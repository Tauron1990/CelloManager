using System.Collections.Immutable;
using TextFragmentLib2.TextProcessing.Parsing;

namespace TextFragmentLib2.TextProcessing.Ast;

public abstract class ExpressionBaseNode : AstNode
{
    public static readonly ExpressionBaseNode Empty = new EmptyNode();

    public static ExpressionBaseNode Create(string literal, bool isReference)
        => new LiteralExpressionNode(literal, isReference);

    public static ExpressionBaseNode Create(OperatorType operatorType, ExpressionBaseNode expression)
        => new UnaryExpressionNode(operatorType, expression);

    public static ExpressionBaseNode Create(string methodName, ImmutableList<ExpressionBaseNode> parameters)
        => new CallExpressionNode(methodName, parameters);

    public static ExpressionBaseNode Create(OperatorType operatorType, ExpressionBaseNode left,
        ExpressionBaseNode right)
        => new BinaryExpressionNode(operatorType, left, right);
    
    public abstract TReturn Visit<TReturn>(ExpressionNodeVisitor<TReturn> visitor);

    public virtual ExpressionBaseNode Merge(ExpressionBaseNode node)
        => new BinaryExpressionNode(OperatorType.Add, this, node);

    protected sealed class EmptyNode : ExpressionBaseNode
    {
        public override void Validate()
        {
        }

        protected override string Format()
            => string.Empty;

        public override TReturn Visit<TReturn>(ExpressionNodeVisitor<TReturn> visitor)
            => throw new InvalidOperationException("An Empty Node sould not be Encounterd");

        public override ExpressionBaseNode Merge(ExpressionBaseNode node)
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