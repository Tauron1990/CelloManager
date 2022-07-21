using System.Collections.Immutable;
using System.Text;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

public sealed class TemplateEntryNode : TemplateNode
{
    public TemplateMatcherNode Match { get; set; } = TemplateMatcherNode.Empty;

    public ImmutableList<AttributeNode> Attributes { get; set; } = ImmutableList<AttributeNode>.Empty;

    public override void Validate()
    {
        if (Match == TemplateMatcherNode.Empty)
            ThrowValidationError("No matcher for Emplate");
        Attributes.ForEach(a => a.Validate());
    }

    protected override string Format()
        => new StringBuilder()
            .Append('{')
            .Append(Match)
            .Append('(')
            .AppendJoin(
                ',',
                Attributes)
            .Append(')')
            .Append('}')
            .ToString();
}