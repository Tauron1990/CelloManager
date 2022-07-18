namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

public sealed class NotMatcherNode : TemplateMatcherNode
{
    public TemplateMatcherNode MatcherNode { get; set; } = Empty;

    protected override string Format()
        => $"!({MatcherNode})";
}