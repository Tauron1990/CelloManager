using TextFragmentLib2.TextProcessing.Parsing;

namespace TextFragmentLib2.TextProcessing.Ast;

public sealed class NameMatchNode : TemplateMatcherNode
{
    public string Name { get; set; } = string.Empty;

    public override void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            ThrowValidationError("No Name for Matcher");
    }

    protected override string Format()
        => Name;

    public override TResult Visit<TResult>(TemplateMatcherVisitor<TResult> visitor) => visitor.VisitNameMatch(this);
}