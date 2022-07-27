using TextFragmentLib2.TextProcessing.Parsing;

namespace TextFragmentLib2.TextProcessing.Ast;

public sealed class TypeMatcherNode : TemplateMatcherNode
{
    public string TypeName { get; set; } = string.Empty;


    public override void Validate()
    {
        if (string.IsNullOrWhiteSpace(TypeName))
            ThrowValidationError("No Type Name");

        if (Type.GetType(
                TypeName,
                true)?.IsAbstract == null)
            ThrowValidationError("Abstract Class");
    }

    protected override string Format()
        => TypeName;

    public override TResult Visit<TResult>(TemplateMatcherVisitor<TResult> visitor) => visitor.VisitTypeMatcher(this);
}