namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

public sealed class NameMatchNode : TemplateMatcherNode
{
    public string Name { get; set; } = string.Empty;

    public bool SimpleExpression { get; set; }
    
    protected override string Format()
        => Name;
}