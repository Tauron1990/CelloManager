using RaiseOfNewWorld.Engine.Data.TextProcessing.Parsing;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

public abstract class TemplateMatcherNode : TemplateNode
{
    public static readonly TemplateMatcherNode Empty = new EmptyNode();

    public abstract TResult Visit<TResult>(TemplateMatcherVisitor<TResult> visitor);

    private sealed class EmptyNode : TemplateMatcherNode
    {
        public override void Validate()
        {
        }

        protected override string Format() => string.Empty;

        public override TResult Visit<TResult>(TemplateMatcherVisitor<TResult> visitor)
            => throw new InvalidOperationException("No Empty Template node Should Encounterd");
    }
}