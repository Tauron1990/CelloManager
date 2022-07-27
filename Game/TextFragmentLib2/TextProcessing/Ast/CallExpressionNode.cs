using System.Collections.Immutable;
using System.Text;
using TextFragmentLib2.TextProcessing.Parsing;

namespace TextFragmentLib2.TextProcessing.Ast;

public sealed class CallExpressionNode : ExpressionBaseNode
{
    public string MethodName { get; }

    public ImmutableList<ExpressionBaseNode> Parameters { get; private set; }

    public CallExpressionNode(string methodName, ImmutableList<ExpressionBaseNode> parameters)
    {
        MethodName = methodName;
        Parameters = parameters;
    }

    public override void Validate()
    {
        if (string.IsNullOrWhiteSpace(MethodName))
            ThrowValidationError("No Method Name Provided");

        Parameters.ForEach(p => p.Validate());
    }

    protected override string Format()
        => new StringBuilder()
            .Append(MethodName)
            .Append('(')
            .AppendJoin(
                ',',
                Parameters)
            .Append(')')
            .ToString();

    public override ExpressionBaseNode Merge(ExpressionBaseNode node)
    {
        if (node is not CallExpressionNode call) return base.Merge(node);

        if (call.MethodName != MethodName) return call;

        for (var i = 0; i < call.Parameters.Count; i++)
            if (i < Parameters.Count)
            {
                var old = Parameters[i];
                Parameters = Parameters.Replace(
                    Parameters[i],
                    old.Merge(call.Parameters[i]));
            }
            else
            {
                Parameters = Parameters.Add(call.Parameters[i]);
            }

        return base.Merge(node);
    }

    public override TReturn Visit<TReturn>(ExpressionNodeVisitor<TReturn> visitor)
        => visitor.VisitCall(this);
}