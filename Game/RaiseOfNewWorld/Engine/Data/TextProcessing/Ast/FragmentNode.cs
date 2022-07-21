namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

public abstract class FragmentNode : AstNode
{
    public abstract TReturn Visit<TReturn>(FragmentNodeVisitor<TReturn> fragmentNodeVisitor);
}