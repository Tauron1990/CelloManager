using System.Collections.Immutable;
using System.Text;

namespace TextFragmentLib2.TextProcessing.Ast;

public abstract class FragmentContainerNode : FragmentNode
{
    public ImmutableList<TextFragmentNode> FragmentNodes { get; }

    protected FragmentContainerNode(ImmutableList<TextFragmentNode> fragmentNodes)
        => FragmentNodes = fragmentNodes;

    protected void FormatFragments(StringBuilder builder)
    {
        foreach (var node in FragmentNodes) builder.AppendLine($"{node}");
    }
}