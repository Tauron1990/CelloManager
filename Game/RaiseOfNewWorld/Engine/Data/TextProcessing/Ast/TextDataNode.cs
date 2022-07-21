using System.Collections.Immutable;
using System.Text;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

public sealed class TextDataNode : FragmentContainerNode
{
    public ImmutableList<TemplateReferenceNode> Templates { get; set; } = ImmutableList<TemplateReferenceNode>.Empty;


    public override void Validate()
    {
        Templates.ForEach(t => t.Validate());
    }

    protected override string Format()
    {
        var builder = new StringBuilder();
        foreach (var template in Templates)
            builder.AppendLine(template.ToString());

        if (Templates.Count != 0)
            builder.AppendLine();

        FormatFragments(builder);

        return builder.ToString();
    }
}