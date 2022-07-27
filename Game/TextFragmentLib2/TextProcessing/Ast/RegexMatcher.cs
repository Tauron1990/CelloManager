using TextFragmentLib2.TextProcessing.Parsing;

namespace TextFragmentLib2.TextProcessing.Ast;

public class RegexMatcherNode : TemplateMatcherNode
{
    public string Regex { get; set; } = string.Empty;

    public override void Validate()
    {
        if (string.IsNullOrWhiteSpace(Regex))
            ThrowValidationError("No Regex Expression");
    }

    protected override string Format()
        => Regex;

    public override TResult Visit<TResult>(TemplateMatcherVisitor<TResult> visitor) => visitor.VisitRegexMatcher(this);
}