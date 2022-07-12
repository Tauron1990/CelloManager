using System.Collections.Immutable;
using System.Text;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

public sealed class TextFragmentNode : FragmentContainerNode
{
    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;

    public ImmutableList<AttributeNode> Attributes { get; set; } = ImmutableList<AttributeNode>.Empty;
    
    protected override string Format()
    {
        var builder = new StringBuilder();

        builder.Append('{');

        if (!string.IsNullOrWhiteSpace(Type))
            builder.Append(Type);
        if (!string.IsNullOrWhiteSpace(Name))
            builder.Append($":{Name}");

        builder.Append('(')
            .AppendJoin(',', Attributes)
            .AppendLine(")");
        
        if (!string.IsNullOrWhiteSpace(Text))
            builder.AppendLine(Text);
        
        FormatFragments(builder);
        
        builder.Append('}');

        return builder.ToString();
    }
}