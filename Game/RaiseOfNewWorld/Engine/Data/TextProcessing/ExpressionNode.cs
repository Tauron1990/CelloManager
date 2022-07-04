namespace RaiseOfNewWorld.Engine.Data.TextProcessing;

public abstract class ExpressionNode<TResult>
{
    public abstract TResult Evaluate(ViewContext context);
}