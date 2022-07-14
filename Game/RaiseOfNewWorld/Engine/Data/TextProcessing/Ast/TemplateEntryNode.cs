using System.Collections.Immutable;
using System.Text;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

public sealed class TemplateEntryNode : AstNode
{
    public TemplateMatcherNode Match { get; set; } = TemplateMatcherNode.Empty;

    public ImmutableList<AttributeNode> Attributes { get; set; } = ImmutableList<AttributeNode>.Empty;
    
    protected override string Format()
    => new StringBuilder()
            .Append('{')
            .Append(Match)
            .Append('(')
            .AppendJoin(',', Attributes)
            .Append(')')
            .Append('}')
            .ToString();
}