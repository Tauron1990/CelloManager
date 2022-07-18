namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

public class RegexMatcherNode : TemplateMatcherNode
{
    public string Regex { get; set; } = string.Empty;

    protected override string Format()
        => Regex;
}