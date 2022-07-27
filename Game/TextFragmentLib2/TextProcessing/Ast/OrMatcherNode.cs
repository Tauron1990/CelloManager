using TextFragmentLib2.TextProcessing.Parsing;

namespace TextFragmentLib2.TextProcessing.Ast;

public sealed class OrMatcherNode : TemplateMatcherNode
{
    public TemplateMatcherNode Left { get; set; } = Empty;

    public TemplateMatcherNode Right { get; set; } = Empty;

    public override void Validate()
    {
        if (Left == Empty)
            ThrowValidationError("No Left Operant");
        if (Right == Empty)
            ThrowValidationError("No Right Operant");

        Left.Validate();
        Right.Validate();
    }

    protected override string Format()
        => $"{Left} or {Right}";

    public override TResult Visit<TResult>(TemplateMatcherVisitor<TResult> visitor) => visitor.VisitOrMatcher(this);
}