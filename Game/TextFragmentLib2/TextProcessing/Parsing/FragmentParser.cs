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
    private static TokenListParserResult<TextToken, TResult> ParseExpression<TResult>(TokenListParser<TextToken, TResult> parser, string input)
    {
        var tokens = FragmentTokenizer.Instance.Tokenize(input.Trim());
        return parser.TryParse(tokens);
    }

    public static TokenListParserResult<TextToken, ExpressionBaseNode> ParseUnary(string input)
        => ParseExpression(FragmentParser.UnaryExpression, input);

    public static TokenListParserResult<TextToken, ExpressionBaseNode> ParseCall(string input)
        => ParseExpression(FragmentParser.CallExpression, input);

    public static TokenListParserResult<TextToken, ExpressionBaseNode> ParseBinary(string input)
        => ParseExpression(FragmentParser.BinaryExpression, input);

    public static TokenListParserResult<TextToken, AttributeNode[]> ParseNodes(string input)
        => ParseExpression(FragmentParser.Attributes, input);
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

    private static TokenListParser<TextToken, TReturn[]> CommaSeperated<TReturn>(
        TokenListParser<TextToken, TReturn> input)
        => input.IgnoreWhitespace().ManyDelimitedBy(Comma.IgnoreWhitespace());    
    
    private static readonly TokenListParser<TextToken, ExpressionBaseNode> Expression = 
        Parse.Ref(() => Parse.OneOf(UnaryExpression.Try(), BinaryExpression.Try(), CallExpression.Try(), Literal))
            .IgnoreWhitespace();
    
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
        from param in CommaSeperated(Expression)
            .Between(OpenPan, ClosePan).IgnoreWhitespace()
        select ExpressionBaseNode.Create(name.ToStringValue(), param.ToImmutableList());

    internal static readonly TokenListParser<TextToken, ExpressionBaseNode> BinaryExpression =
        Parse.OneOf(UnaryExpression.Try(), CallExpression.Try(), Literal).IgnoreWhitespace()
            .Chain(
                Operator.IgnoreWhitespace(),
                Expression.IgnoreWhitespace(),
                (operatorType, left, right)
                    => new BinaryExpressionNode(operatorType, left, right));
        // from left in Parse.OneOf(UnaryExpression.Try(), CallExpression.Try(), Literal).IgnoreWhitespace()
        // from op in Operator.IgnoreWhitespace()
        // from right in Expression.IgnoreWhitespace()
        // select ExpressionBaseNode.Create(op, left, right);
        

    private static TResult ParserBase<TResult>(TokenListParser<TextToken, TResult> parser, string input)
    {
        var tokens = FragmentTokenizer.Instance.Tokenize(input.Trim());
        return parser.Parse(tokens);
    }


    public static ExpressionBaseNode ParseExpression(string input)
        => ParserBase(Expression, input);

    internal static readonly TokenListParser<TextToken, AttributeNode[]> Attributes =
        CommaSeperated(
            from name in Token.EqualTo(TextToken.Identifer).IgnoreWhitespace()
            from assign in Token.EqualTo(TextToken.Assign).IgnoreWhitespace()
            from expr in Expression
            select new AttributeNode(name.ToStringValue(), expr)
        ).Between(OpenPan, ClosePan);

    private static readonly TokenListParser<TextToken, TypeRepesentation> TypeReprensentation =
        from name in Token.EqualTo(TextToken.Identifer).IgnoreWhitespace()
        from parameter in
        (
            from dp in Token.EqualTo(TextToken.DoublePoint)
            from parameterToken in Token.EqualTo(TextToken.Identifer)
            select parameterToken.ToStringValue()
        ).Try().OptionalOrDefault(string.Empty)
        select new TypeRepesentation(name.ToStringValue(), parameter);

    private static readonly TokenListParser<TextToken, string> FragmentContent =
        from content in Token.Matching<TextToken>(t => t != TextToken.Closebrace, "Fragment Conten").Many()
        from end in Token.EqualTo(TextToken.Closebrace)
        select string.Concat(content.Select(t => t.ToStringValue()));

    private static readonly TokenListParser<TextToken, Func<string, ImmutableList<TextFragmentNode>, TextFragmentNode>>
        HeaderParser =
            Parse.OneOf(
                (
                    from name in Token.EqualTo(TextToken.Identifer)
                    from typeRep in TypeReprensentation
                    from attribute in Attributes.Try().OptionalOrDefault(Array.Empty<AttributeNode>())
                    select new Func<string, ImmutableList<TextFragmentNode>, TextFragmentNode>(
                        (content, fragments) => new TextFragmentNode(
                            name.ToStringValue(),
                            typeRep,
                            content,
                            attribute.ToImmutableList(),
                            fragments))
                ).Try(),
                (
                    from name in Token.EqualTo(TextToken.Identifer)
                    from attribute in Attributes.Try().OptionalOrDefault(Array.Empty<AttributeNode>())
                    select new Func<string, ImmutableList<TextFragmentNode>, TextFragmentNode>(
                        (content, fragments) => new TextFragmentNode(
                            name.ToStringValue(),
                            TypeRepesentation.Empty,
                            content,
                            attribute.ToImmutableList(),
                            fragments))
                ).Try(),
                Parse.Return<TextToken, Func<string, ImmutableList<TextFragmentNode>, TextFragmentNode>>(
                    (content, fragments) => new TextFragmentNode(
                        string.Empty,
                        TypeRepesentation.Empty,
                        content,
                        ImmutableList<AttributeNode>.Empty,
                        fragments))
            );


    private static readonly TokenListParser<TextToken, TextFragmentNode> FragmentNodeParser =
        Parse.OneOf
        (
            from open in Token.EqualTo(TextToken.OpenBrace)
            from close in Token.EqualTo(TextToken.Closebrace)
            select new TextFragmentNode(
                string.Empty,
                TypeRepesentation.Empty,
                string.Empty,
                ImmutableList<AttributeNode>.Empty,
                ImmutableList<TextFragmentNode>.Empty)
        );
}