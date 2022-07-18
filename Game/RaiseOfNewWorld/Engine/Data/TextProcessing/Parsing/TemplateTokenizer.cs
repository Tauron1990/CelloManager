using System.Collections.Immutable;
using System.Text;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Parsing;

public sealed class TemplateTokenizer : TokenizerBase<TemplateToken>
{
    private static TemplateToken CreateText(string text, int position)
        => new(text, string.IsNullOrEmpty(text) ? TemplateTokentype.Eof : TemplateTokentype.Text, position);

    private static readonly ImmutableDictionary<char, TokenBuilder<TemplateToken>> Tokens 
        = ImmutableDictionary<char, TokenBuilder<TemplateToken>>.Empty
            .Add('{', (txt, pos) => new TemplateToken(txt, TemplateTokentype.OpenTemplate, pos))
            .Add('}', (txt, pos) => new TemplateToken(txt, TemplateTokentype.CloseTemplate, pos))
            .Add(':' , (txt, pos) => new TemplateToken(txt, TemplateTokentype.TemplateMatchSeperator, pos))
            .Add('!', (txt, pos) => new TemplateToken(txt, TemplateTokentype.MatchNot, pos))
            .Add('(', (txt, pos) => new TemplateToken(txt, TemplateTokentype.OpenExpression, pos))
            .Add(')', (txt, pos) => new TemplateToken(txt, TemplateTokentype.CloseExpression, pos));

    public TemplateTokenizer(string input) 
        : base(input, Tokens, CreateText) { }

    public Tokenizer CreateTokenizer()
    {
        var builder = new StringBuilder();
        
        var token = GetAndIncement();
        if (token.Type != TemplateTokentype.Text)
            throw new InvalidOperationException($"Invalid Token for Attribute Tokenizer Creation: {token}");

        builder.Append(token.Text);

        var nextToken = Get();
        while (nextToken.Type is not TemplateTokentype.Eof or TemplateTokentype.CloseTemplate)
        {
            Incremnt();
            builder.Append(nextToken.Text);

            nextToken = Get();
        }
        
        return new Tokenizer(token.Text);
    }
}