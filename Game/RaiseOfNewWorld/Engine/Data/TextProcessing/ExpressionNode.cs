namespace RaiseOfNewWorld.Engine.Data.TextProcessing;

public abstract class ExpressionNode<TResult>
{   
    protected abstract class ExpressionParser
    {
        private readonly string _input;

        protected ExpressionParser(string input) => _input = input;

        protected abstract ExpressionNode<TResult> ParseFabricator(string input);
    }

    public abstract TResult Evaluate(ViewContext context);
}