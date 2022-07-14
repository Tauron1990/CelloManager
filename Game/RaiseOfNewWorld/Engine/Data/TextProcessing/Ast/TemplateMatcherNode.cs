namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

public abstract class TemplateMatcherNode : AstNode
{
    public static readonly TemplateMatcherNode Empty = new EmptyNode();
    
    private sealed class EmptyNode : TemplateMatcherNode
    {
        protected override string Format() => string.Empty;
    }
}