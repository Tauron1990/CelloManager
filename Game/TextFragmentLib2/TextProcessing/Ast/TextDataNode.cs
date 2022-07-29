using System.Collections.Immutable;
using System.Text;
using TextFragmentLib2.TextProcessing.Parsing;

namespace TextFragmentLib2.TextProcessing.Ast;

public sealed class TextDataNode : FragmentContainerNode
{
    public ImmutableList<TemplateReferenceNode> Templates { get; }


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

    public override TReturn Visit<TReturn>(FragmentNodeVisitor<TReturn> fragmentNodeVisitor)
        => fragmentNodeVisitor.VisitTextData(this);

    public TextDataNode(ImmutableList<TextFragmentNode> fragmentNodes, ImmutableList<TemplateReferenceNode> templates) : base(fragmentNodes)
        => Templates = templates;
}