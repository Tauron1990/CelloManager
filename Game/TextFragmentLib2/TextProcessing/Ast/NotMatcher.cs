using TextFragmentLib2.TextProcessing.Parsing;

namespace TextFragmentLib2.TextProcessing.Ast;

public sealed class NotMatcherNode : TemplateMatcherNode
{
    public TemplateMatcherNode MatcherNode { get; set; } = Empty;

    public override void Validate()
    {
        if (MatcherNode == Empty)
            ThrowValidationError("No matcher Provided");

        MatcherNode.Validate();
    }

    protected override string Format()
        => $"!({MatcherNode})";

    public override TResult Visit<TResult>(TemplateMatcherVisitor<TResult> visitor) => visitor.VisitNot(this);
}