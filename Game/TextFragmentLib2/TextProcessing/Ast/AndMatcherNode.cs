using TextFragmentLib2.TextProcessing.Parsing;

namespace TextFragmentLib2.TextProcessing.Ast;

public sealed class AndMatcherNode : TemplateMatcherNode
{
    public TemplateMatcherNode Left { get; set; } = Empty;

    public TemplateMatcherNode Right { get; set; } = Empty;

    public override void Validate()
    {
        if (Left == Empty)
            ThrowValidationError("No Left Matcher");
        if (Right == Empty)
            ThrowValidationError("No Right Matcher");

        Left.Validate();
        Right.Validate();
    }

    protected override string Format()
        => $"{Left} and {Right}";

    public override TResult Visit<TResult>(TemplateMatcherVisitor<TResult> visitor) => visitor.VisitAndMatcher(this);
}