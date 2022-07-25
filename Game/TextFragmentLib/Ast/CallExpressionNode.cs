using System.Collections.Immutable;

namespace TextFragmentLib.Ast;

public sealed class CallExpressionNode : ExpressionNode
{
    public CallExpressionNode(string methodName, ImmutableArray<ExpressionNode> parameters)
    {
        MethodName = methodName;
        Parameters = parameters;
    }

    public string MethodName { get; }

    public ImmutableArray<ExpressionNode> Parameters { get; }

    public override void Validate()
    {
        if (string.IsNullOrWhiteSpace(MethodName))
            ThrowValidationError("Nod Method Name");
    }

    protected override string Format()
        => $"{MethodName}({string.Join(',', Parameters)})";
}