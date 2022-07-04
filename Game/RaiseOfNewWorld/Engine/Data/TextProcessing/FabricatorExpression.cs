namespace RaiseOfNewWorld.Engine.Data.TextProcessing;

public abstract class FabricatorExpression<TResult> : ExpressionNode<TResult>
{
    public sealed record FabricatorData(string Input, string Name);
    
    private readonly Func<ViewContext, FabricatorData, TResult> _creationFunc;

    public string Input { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    
    public FabricatorExpression(Func<ViewContext, FabricatorData, TResult> creationFunc) => _creationFunc = creationFunc;

    public override TResult Evaluate(ViewContext context)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new InvalidOperationException("No Name for Fabricator");

        return _creationFunc(context, new FabricatorData(Input, Name));
    }
}