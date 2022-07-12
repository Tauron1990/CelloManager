namespace RaiseOfNewWorld.Engine.Data.TextProcessingOld;

public abstract class OperatorExpression<TResult> : ExpressionNode<TResult>
{
    private readonly Func<ViewContext, TResult, TResult, TResult> _operatorFunc;
    
    public ExpressionNode<TResult>? Right { get; set; }

    public ExpressionNode<TResult>? Left { get; set; }
    
    protected OperatorExpression(Func<ViewContext, TResult?, TResult?, TResult> operatorFunc) => _operatorFunc = operatorFunc;

    public override TResult Evaluate(ViewContext context)
    {
        if (Right is null || Left is null)
            throw new InvalidOperationException($"One Operatr is null l{Left} r{Right}");
        
        return _operatorFunc(context, Right.Evaluate(context), Left.Evaluate(context));
    }
}

public abstract class AddExpression<TResult> : OperatorExpression<TResult>
{
    protected AddExpression(Func<ViewContext, TResult?, TResult?, TResult> operatorFunc) : base(operatorFunc)
    {
    }
}

public abstract class SubstractExpression<TResult> : OperatorExpression<TResult>
{
    protected SubstractExpression(Func<ViewContext, TResult?, TResult?, TResult> operatorFunc) : base(operatorFunc)
    {
    }
}