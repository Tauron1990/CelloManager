using RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing;

public abstract class FragmentNodeVisitor<TReturn>
{
    public TReturn Accept(FragmentNode node)
        => node.Visit(this);

    public abstract TReturn VisitTextFragment(TextFragmentNode textFragmentNode);

    public abstract TReturn VisitTextData(TextDataNode textDataNode);
}