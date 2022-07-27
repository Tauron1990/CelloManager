using System.Collections.Concurrent;
using System.Collections.Immutable;
using TextFragmentLib2.TextProcessing.Ast;

namespace TextFragmentLib2.TextProcessing.ParsingOld;

public sealed class TemplateParser
{
    private static readonly ConcurrentDictionary<string, TemplateReferenceNode> Templates = new();
    private readonly string _source;
    private readonly TemplateTokenizer _template;

    private TemplateParser(string source, string template)
    {
        _source = source;
        _template = new TemplateTokenizer(template);
    }

    private TemplateReferenceNode ParseTemplate()
        => new()
        {
            Source = _source,
            Entrys = ParseEntrys().ToImmutableList()
        };

    private IEnumerable<TemplateEntryNode> ParseEntrys()
    {
        TemplateToken token;

        do
        {
            token = _template.GetAndIncement();
            ValidateToken(
                token,
                TemplateTokentype.OpenTemplate);

            yield return new TemplateEntryNode
            {
                Match = ParseMatcher(),
                Attributes = TextParser.ReadAttributes(_template.CreateTokenizer()).ToImmutableList()
            };

            token = _template.GetAndIncement();
            ValidateToken(
                token,
                TemplateTokentype.CloseTemplate);
        } while (token.Type != TemplateTokentype.Eof);
    }

    private TemplateMatcherNode ParseMatcher()
    {
        var token = _template.Get();
        var leftNode = token.Type switch
        {
            TemplateTokentype.MatchNot => ParseNotMatcher(),
            TemplateTokentype.Text => ParseSimpleMatcher(),
            _ => throw new InvalidOperationException($"Invalid Token {token}")
        };

        token = _template.Get();
        return token.Text switch
        {
            "and" => new AndMatcherNode { Left = leftNode, Right = ParseMatcher() },
            "or" => new OrMatcherNode { Left = leftNode, Right = ParseMatcher() },
            _ => leftNode
        };
    }

    private TemplateMatcherNode ParseNotMatcher()
    {
        ValidateToken(
            _template.GetAndIncement(),
            TemplateTokentype.MatchNot);
        ValidateToken(
            _template.GetAndIncement(),
            TemplateTokentype.OpenExpression);

        var node = new NotMatcherNode { MatcherNode = ParseMatcher() };

        ValidateToken(
            _template.GetAndIncement(),
            TemplateTokentype.CloseExpression);
        return node;
    }

    private TemplateMatcherNode ParseSimpleMatcher()
    {
        var token = _template.GetAndIncement();
        ValidateToken(
            token,
            TemplateTokentype.Text);

        var token2 = _template.Get();
        if (token2.Type != TemplateTokentype.TemplateMatchSeperator) return new NameMatchNode { Name = token.Text };

        _template.Incremnt();
        token2 = _template.GetAndIncement();
        ValidateToken(
            token2,
            TemplateTokentype.Text);

        return token.Text switch
        {
            "name" => new NameMatchNode { Name = token2.Text },
            "regx" => new RegexMatcherNode { Regex = token2.Text },
            _ => throw new InvalidOperationException($"Matcher Type Unkowen: {token}")
        };
    }

    private static void ValidateToken(TemplateToken textToken, TemplateTokentype expected)
    {
        if (textToken.Type == expected)
            return;

        ThrowInvalidToken(
            textToken,
            expected);
    }

    private static void ThrowInvalidToken(TemplateToken textToken, TemplateTokentype expected)
        => throw new InvalidOperationException($"Invalid Token {textToken} :Expeced:{expected}");

    public static TemplateReferenceNode Parse(string fileName)
        => Templates.GetOrAdd(
            fileName,
            relativeFileName =>
                new TemplateParser(
                    fileName,
                    Pools.ReadFile(relativeFileName)).ParseTemplate());
}