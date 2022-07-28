using System.Collections.Immutable;
using System.Linq.Expressions;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using TextFragmentLib2.TextProcessing.Ast;

namespace TextFragmentLib2.TextProcessing.Parsing;

#if DEBUG
public static class FragmentParserDebug
{
    private static ExpressionBaseNode ParseExpression(TokenListParser<TextToken, ExpressionBaseNode> parser, string input)
    {
        var tokens = FragmentTokenizer.Instance.Tokenize(input.Trim());
        return parser.Parse(tokens);
    }

    public static ExpressionBaseNode ParseUnary(string input)
        => ParseExpression(FragmentParser.UnaryExpression, input);

    public static ExpressionBaseNode ParseCall(string input)
        => ParseExpression(FragmentParser.CallExpression, input);

    public static ExpressionBaseNode ParseBinary(string input)
        => ParseExpression(FragmentParser.BinaryExpression, input);
}
#endif

public static class FragmentParser
{
    public static TokenListParser<TextToken, TResult> IgnoreWhitespace<TResult>(this TResult input)
        => Token.EqualTo(TextToken.Whitespace).Many().OptionalOrDefault(Array.Empty<Token<TextToken>>()).Select(_ => input);
    
    public static TokenListParser<TextToken, TResult> IgnoreWhitespace<TResult>(this TokenListParser<TextToken, TResult> input)
        => input.Then(IgnoreWhitespace);

    private static readonly TokenListParser<TextToken, Token<TextToken>> OpenPan =
        Token.EqualTo(TextToken.OpenPan).IgnoreWhitespace();
    
    private static readonly TokenListParser<TextToken, Token<TextToken>> ClosePan =
        Token.EqualTo(TextToken.ClosePan).IgnoreWhitespace();

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

    private static readonly TokenListParser<TextToken, ExpressionBaseNode> Literal =
        Token.EqualTo(TextToken.Template)
            .Then(_ => Token.EqualTo(TextToken.Identifer))
            .Select(t => ExpressionBaseNode.Create(t.ToStringValue(), true))
            .Try()
            .Or(Token.EqualTo(TextToken.Identifer)
                .Select(t => ExpressionBaseNode.Create(t.ToStringValue(), false)));

    internal static readonly TokenListParser<TextToken, ExpressionBaseNode> UnaryExpression =
        from op in Operator.IgnoreWhitespace()
        from exp in Expression.IgnoreWhitespace()
        select ExpressionBaseNode.Create(op, exp);

    internal static readonly TokenListParser<TextToken, ExpressionBaseNode> CallExpression =
        from name in Token.EqualTo(TextToken.Identifer).IgnoreWhitespace()
        from param in Expression.IgnoreWhitespace()
            .Then(n => Comma.Select(_ => n).IgnoreWhitespace().OptionalOrDefault(n)).Many()
            .Between(OpenPan, ClosePan).IgnoreWhitespace()
        select ExpressionBaseNode.Create(name.ToStringValue(), param.ToImmutableList());

    internal static readonly TokenListParser<TextToken, ExpressionBaseNode> BinaryExpression =
        from left in Parse.OneOf(UnaryExpression.Try(), CallExpression.Try(), Literal).IgnoreWhitespace()
        from op in Operator.IgnoreWhitespace()
        from right in Expression.IgnoreWhitespace()
        select ExpressionBaseNode.Create(op, left, right);

    private static readonly TokenListParser<TextToken, ExpressionBaseNode> Expression = 
        Parse.Ref(() => Parse.OneOf(UnaryExpression.Try(), BinaryExpression.Try(), CallExpression.Try(), Literal))
            .IgnoreWhitespace();

    public static TResult ParserBase<TResult>(TokenListParser<TextToken, TResult> parser, string input)
    {
        var tokens = FragmentTokenizer.Instance.Tokenize(input.Trim());
        return parser.Parse(tokens);
    }


    public static ExpressionBaseNode ParseExpression(string input)
        => ParserBase(Expression, input);
}