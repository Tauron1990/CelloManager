using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using TextFragmentLib2.TextProcessing.Ast;

namespace TextFragmentLib2.TextProcessing.Parsing;

public static class FragmentParser
{
    private static TokenListParser<TextToken, TResult> IgnoreWhitespace<TResult>(this TResult input)
        => Token.EqualTo(TextToken.Whitespace).Many().OptionalOrDefault(Array.Empty<Token<TextToken>>()).Select(_ => input);
    
    private static TokenListParser<TextToken, TResult> IgnoreWhitespace<TResult>(this TokenListParser<TextToken, TResult> input)
        => input.Then(IgnoreWhitespace);

    private static readonly TokenListParser<TextToken, Token<TextToken>> OpenPan =
        Token.EqualTo(TextToken.OpenPan).IgnoreWhitespace();

    private static readonly TokenListParser<TextToken, OperatorType> Operator =
        from tt in Token.Matching<TextToken>(
            k => k is TextToken.And or TextToken.Or or TextToken.Plus or TextToken.Minus
                or TextToken.Not or TextToken.DoublePoint or TextToken.Mult or TextToken.Equal or TextToken.NotEqual,
            "Operator Match")
        select tt.Kind switch
        {
            TextToken.Not => OperatorType.Not,
            TextToken.NotEqual => OperatorType.NotEqual,
            TextToken.Equal => OperatorType.Equal,
            TextToken.DoublePoint => OperatorType.Div,
            TextToken.Mult => OperatorType.Mult,
            TextToken.Minus => OperatorType.Subtract,
            TextToken.Plus => OperatorType.Add,
            TextToken.Or => OperatorType.Or,
            TextToken.And => OperatorType.And,
            _ => OperatorType.None
        };

    private static readonly TokenListParser<TextToken, Token<TextToken>> Comma = Token.EqualTo(TextToken.Comma);

    private static readonly TokenListParser<TextToken, UnaryAttributeValue> UnaryExpression =
        from op in Operator.IgnoreWhitespace()
        from exp in Expression
        select new UnaryAttributeValue(op, exp);

    private static readonly TokenListParser<TextToken, CallAttributeValue> CallExpression =
        from name in Token.EqualTo(TextToken.Identifer).IgnoreWhitespace()
        from param in Expression
            .Then(n => Comma.Select(_ => n).IgnoreWhitespace()).Many()
            .OptionalOrDefault(Array.Empty<AttributeValueNode>())
            .Between().IgnoreWhitespace()
        select new 
        

    private static readonly TokenListParser<TextToken, AttributeValueNode> Expression = null;
}