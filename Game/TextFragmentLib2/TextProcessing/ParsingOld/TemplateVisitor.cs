using TextFragmentLib2.TextProcessing.Ast;

namespace TextFragmentLib2.TextProcessing.ParsingOld;

public abstract class TemplateMatcherVisitor<TResult>
{
    public TResult Accept(TemplateMatcherNode node) => node.Visit(this);

    public abstract TResult VisitAndMatcher(AndMatcherNode andMatcherNode);

    public abstract TResult VisitNameMatch(NameMatchNode nameMatchNode);

    public abstract TResult VisitNot(NotMatcherNode notMatcherNode);

    public abstract TResult VisitOrMatcher(OrMatcherNode orMatcherNode);

    public abstract TResult VisitRegexMatcher(RegexMatcherNode regexMatcherNode);

    public abstract TResult VisitTypeMatcher(TypeMatcherNode typeMatcherNode);
}