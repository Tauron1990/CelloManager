using RaiseOfNewWorld.Engine.Data.TextProcessing.Parsing;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

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