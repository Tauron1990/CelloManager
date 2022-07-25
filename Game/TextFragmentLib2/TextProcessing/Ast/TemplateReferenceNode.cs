using System.Collections.Immutable;

namespace TextFragmentLib2.TextProcessing.Ast;

public sealed class TemplateReferenceNode : TemplateNode
{
    public string Source { get; set; } = string.Empty;

    public ImmutableList<TemplateEntryNode> Entrys { get; set; } = ImmutableList<TemplateEntryNode>.Empty;

    public override void Validate()
    {
        Entrys.ForEach(tn => tn.Validate());
    }

    protected override string Format()
        => $"${Source}";
}