namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

public sealed class OrmatcherNode : TemplateMatcherNode
{
    public TemplateMatcherNode Left { get; set; } = Empty;

    public TemplateMatcherNode Right { get; set; } = Empty;

    protected override string Format()
        => $"{Left} or {Right}";
}