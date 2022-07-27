using System.Buffers;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace TextFragmentLib2.TextProcessing.Parsing;

public static class FragmentTokenizer
{
    private static readonly char[] Except = { '-', '\\', '+', '{', '}', '(', ')', ':', ',', '=', '!' };
    
    public static readonly Tokenizer<TextToken> Instance =
        new TokenizerBuilder<TextToken>()
            .Match(Span.WhiteSpace.AtLeastOnce(), TextToken.Whitespace)
            .Match(Character.EqualTo('-'), TextToken.Minus)
            .Match(Character.EqualTo('\\'), TextToken.Backslash)
            .Match(Character.EqualTo('+'), TextToken.Plus)
            .Match(Character.EqualTo('$'), TextToken.Template)
            .Match(Character.EqualTo('{'), TextToken.OpenBrace)
            .Match(Character.EqualTo('}'), TextToken.Closebrace)
            .Match(Character.EqualTo('('), TextToken.OpenPan)
            .Match(Character.EqualTo(')'), TextToken.ClosePan)
            .Match(Character.EqualTo(':'), TextToken.DoublePoint)
            .Match(Character.EqualTo(','), TextToken.Comma)
            .Match(Character.EqualTo('!'), TextToken.Not)
            .Match(Character.EqualTo('*'), TextToken.Mult)
            .Match(Span.EqualTo("=="), TextToken.Equal)
            .Match(Span.EqualTo("!="), TextToken.NotEqual)
            .Match(Span.EqualTo("and"), TextToken.And)
            .Match(Span.EqualTo("or"), TextToken.Or)
            .Match(Identifier.CStyle, TextToken.Identifer)
            .Match(Character.Numeric.Many(), TextToken.Identifer)
            .Match(Character.Except(c => Except.Contains(c), "Text Value").Many(), TextToken.Text)
            .Build();
}