namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

public abstract class AstNode
{
    public virtual void Validate(){}

    protected abstract string Format();

    public override string ToString()
        => Format();
}