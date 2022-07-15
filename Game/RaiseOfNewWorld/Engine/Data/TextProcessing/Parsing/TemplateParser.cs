using System.Collections.Concurrent;
using System.Collections.Immutable;
using RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Parsing;

public sealed class TemplateParser
{
    private readonly string _source;
    private readonly TemplateTokenizer _template;
    private static readonly ConcurrentDictionary<string, TemplateReferenceNode> Templates = new();

    private TemplateParser(string source, string template)
    {
        _source = source;
        _template = new TemplateTokenizer(template);
    }

    private TemplateReferenceNode ParseTemplate()
    {
        return new TemplateReferenceNode
        {
            Source = _source,
            Entrys = ParseEntrys().ToImmutableList()
        };
    }

    private IEnumerable<TemplateEntryNode> ParseEntrys()
    {
        TemplateToken token = default;
        
        do
        {
            token = _template.GetAndIncement();
            ValidateToken(token, TemplateTokentype.OpenTemplate);

            yield return new TemplateEntryNode
            {
                Match = ParseMatcher(),
                Attributes = TextParser.ReadAttributes(_template.CreateTokenizer()).ToImmutableList()
            };
            
            token = _template.GetAndIncement();
            ValidateToken(token, TemplateTokentype.CloseTemplate);
        } while (token.Type != TemplateTokentype.Eof);
    }

    private TemplateMatcherNode ParseMatcher()
    {
        var token = _template.GetAndIncement();
        ValidateToken(token, TemplateTokentype.Text);

        var token2 = _template.Get();
        if (token2.Type == TemplateTokentype.TemplateMatchSeperator)
        {
            
        }
        else
        {
            
        }
    }
    
    private static void ValidateToken(TemplateToken textToken, TemplateTokentype expected)
    {
        if(textToken.Type == expected)
            return;

        ThrowInvalidToken(textToken, expected);
    }

    private static void ThrowInvalidToken(TemplateToken textToken, TemplateTokentype expected) 
        => throw new InvalidOperationException($"Invalid Token {textToken} :Expeced:{expected}");

    public static TemplateReferenceNode Parse(ContentManager contentManager, string fileName)
        => Templates.GetOrAdd(fileName,
            relativeFileName =>
                new TemplateParser(fileName, contentManager.ReadFile(relativeFileName)).ParseTemplate());
}