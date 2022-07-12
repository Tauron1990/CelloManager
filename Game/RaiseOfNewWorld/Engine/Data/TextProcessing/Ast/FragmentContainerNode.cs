using System.Collections.Immutable;
using System.Text;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

public abstract class FragmentContainerNode : AstNode
{
    public ImmutableList<TextFragmentNode> FragmentNodes { get; set; } = ImmutableList<TextFragmentNode>.Empty;

    protected void FormatFragments(StringBuilder builder)
    {
        foreach (var node in FragmentNodes) builder.AppendLine($"{node}");
    }
}