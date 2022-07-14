using System.Collections.Immutable;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Parsing;

public sealed class TemplateTokenizer : TokenizerBase<TemplateToken>
{
    private static TemplateToken CreateText(string text, int position)
        => new(text, string.IsNullOrEmpty(text) ? TemplateTokentype.Eof : TemplateTokentype.Text, position);

    private static readonly ImmutableDictionary<char, TokenBuilder<TemplateToken>> Tokens 
        = ImmutableDictionary<char, TokenBuilder<TemplateToken>>.Empty
            .Add('{', (txt, pos) => new TemplateToken(txt, TemplateTokentype.OpenTemplate, pos))
            .Add('}', (txt, pos) => new TemplateToken(txt, TemplateTokentype.CloseTemplate, pos))
            .Add(':' );

    public TemplateTokenizer(string input) 
        : base(input, Tokens, CreateText) { }

    public Tokenizer CreateTokenizer()
    {
        var token = GetAndIncement();
        if (token.Type != TemplateTokentype.Text)
            throw new InvalidOperationException($"Invalid Token for Attribute Tokenizer Creation: {token}");

        return new Tokenizer(token.Text);
    }
}