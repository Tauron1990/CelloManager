using System.Collections.Immutable;

namespace TextFragmentLib2.TextProcessing.ParsingOld;

public sealed class Tokenizer : TokenizerBase<TextToken>
{
    private static readonly ImmutableDictionary<char, TokenBuilder<TextToken>> Tokens = CreateTokens();

    public Tokenizer(string input)
        : base(
            input,
            Tokens,
            CreateText)
    {
    }

    private static TextToken CreateText(string? text, int position)
        => string.IsNullOrEmpty(text)
            ? TextToken.Eof
            : new TextToken(
                text,
                TokenType.Text,
                position);

    private static ImmutableDictionary<char, TokenBuilder<TextToken>> CreateTokens()
        => ImmutableDictionary<char, TokenBuilder<TextToken>>.Empty
            .Add(
                '-',
                static (txt, pos) => new TextToken(
                    txt,
                    TokenType.Minus,
                    pos))
            .Add(
                '+',
                static (txt, pos) => new TextToken(
                    txt,
                    TokenType.Plus,
                    pos))
            .Add(
                '$',
                static (txt, pos) => new TextToken(
                    txt,
                    TokenType.Template,
                    pos))
            .Add(
                '{',
                static (txt, pos) => new TextToken(
                    txt,
                    TokenType.OpenFragment,
                    pos))
            .Add(
                '}',
                static (txt, pos) => new TextToken(
                    txt,
                    TokenType.CloseFragment,
                    pos))
            .Add(
                '(',
                static (txt, pos) => new TextToken(
                    txt,
                    TokenType.OpenAttribute,
                    pos))
            .Add(
                ')',
                static (txt, pos) => new TextToken(
                    txt,
                    TokenType.CloseAttribute,
                    pos))
            .Add(
                ':',
                static (txt, pos) => new TextToken(
                    txt,
                    TokenType.AttributeValueSeperator,
                    pos))
            .Add(
                ',',
                static (txt, pos) => new TextToken(
                    txt,
                    TokenType.AttributeSeperator,
                    pos));
}