namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

public abstract class AstNode
{
    public abstract void Validate();

    protected abstract string Format();

    public override string ToString()
        => Format();

    protected void ThrowValidationError(string message)
        => throw new InvalidOperationException($"{GetType()} -- {message}");
}