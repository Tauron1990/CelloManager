using System.Collections.Immutable;
using System.Text;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

public sealed class TextFragmentNode : FragmentContainerNode
{
    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;

    public ImmutableList<AttributeNode> Attributes { get; set; } = ImmutableList<AttributeNode>.Empty;

    public void Intigrate(IEnumerable<AttributeNode> attributes)
    {
        foreach (var attributeNode in attributes)
        {
            var element = Attributes.Find(n => n.Name == attributeNode.Name);
            if (element is null)
                Attributes = Attributes.Add(attributeNode);
            else
                element.Value = element.Value.Merge(attributeNode.Value);
        }
    }
    
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