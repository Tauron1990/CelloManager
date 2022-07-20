using RaiseOfNewWorld.Engine.Data.TextProcessing.Parsing;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

public class RegexMatcherNode : TemplateMatcherNode
{
    public string Regex { get; set; } = string.Empty;

    public override void Validate()
    {
        if(string.IsNullOrWhiteSpace(Regex))
            ThrowValidationError("No Regex Expression");
    }

    protected override string Format()
        => Regex;

    public override TResult Visit<TResult>(TemplateMatcherVisitor<TResult> visitor) => visitor.VisitRegexMatcher(this);
}