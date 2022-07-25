using TextFragmentLib2.TextProcessing.Ast;

namespace TextFragmentLib2.TextProcessing.ParsingOld;

public abstract class FragmentNodeVisitor<TReturn>
{
    public TReturn Accept(FragmentNode node)
        => node.Visit(this);

    public abstract TReturn VisitTextFragment(TextFragmentNode textFragmentNode);

    public abstract TReturn VisitTextData(TextDataNode textDataNode);
}