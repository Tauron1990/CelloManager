using System.Collections.Immutable;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

public sealed class TemplateReferenceNode : AstNode
{
    public string Source { get; set; } = string.Empty;

    public ImmutableList<TemplateEntryNode> Entrys { get; set; } = ImmutableList<TemplateEntryNode>.Empty;
    
    protected override string Format()
        => $"${Source}";
}