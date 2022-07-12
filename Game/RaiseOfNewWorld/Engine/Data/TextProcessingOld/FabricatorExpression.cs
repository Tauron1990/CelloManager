namespace RaiseOfNewWorld.Engine.Data.TextProcessingOld;

public abstract class FabricatorExpression<TResult> : ExpressionNode<TResult>
{
    private readonly Func<ViewContext, TResult> _creationFunc;

    protected FabricatorExpression(Func<ViewContext, TResult> creationFunc) => _creationFunc = creationFunc;

    public override TResult Evaluate(ViewContext context) => _creationFunc(context);
}