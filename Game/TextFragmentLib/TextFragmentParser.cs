using System.Collections.Immutable;
using Pidgin;
using TextFragmentLib.Ast;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace TextFragmentLib;

public class TextFragmentParser
{
    private static Parser<char, T> Tok<T>(Parser<char, T> p)
        => Try(p).Before(SkipWhitespaces);

    private static Parser<char, char> Tok(char value) => Tok(Char(value));
    private static Parser<char, string> Tok(string value) => Tok(String(value));
    
    private static readonly Parser<char, char> Comma = Tok(',');
    private static readonly Parser<char, char> OpenParen = Tok('(');
    private static readonly Parser<char, char> CloseParen = Tok(')');
    private static readonly Parser<char, char> Dot = Tok('.');
    private static readonly Parser<char, char> BraceOpen = Tok('{');
    private static readonly Parser<char, char> BraceClose = Tok('}');
    private static readonly Parser<char, char> DoublePoint = Tok(':');
    private static readonly Parser<char, string> Page = Tok("@@@");
    private static readonly Parser<char, char> ExclamationMark = Tok('!');
    private static readonly Parser<char, char> Plus = Tok('+');
    private static readonly Parser<char, char> Minus = Tok('-');
    private static readonly Parser<char, char> Multy = Tok('*');
    private static readonly Parser<char, string> And = Tok("and");
    private static readonly Parser<char, string> Or = Tok("or");

    private static readonly Parser<char, string> Text =
        Tok(
            from first in Letter
            from rest in OneOf(
                LetterOrDigit,
                Char('_')).ManyString()
            select first + rest);

    #region Expresssion

    private static readonly Parser<char, string> Operator = OneOf(
        Multy.Map(c => c.ToString()),
        Minus.Map(c => c.ToString()),
        Plus.Map(c => c.ToString()),
        DoublePoint.Map(c => c.ToString()),
        ExclamationMark.Map(c => c.ToString()),
        And,
        Or);

    static Parser<char, ImmutableArray<T>> CommaSeparated<T>(Parser<char, T> p)
        => Try(p.SeparatedAtLeastOnce(Comma).Select(x => x.ToImmutableArray()))
            .Or(Return(ImmutableArray<T>.Empty));

    private static readonly Parser<char, CallExpressionNode> Call =
        Rec(
            () => from methodName in Text
                from a in CommaSeparated(Expression).Between(OpenParen, CloseParen)
                select new CallExpressionNode(
                    methodName,
                    a)
        )
            .Labelled("CallExpression");

    private static BinaryOperationType CreateOperatorType(string c)
        => c switch
        {
            "+" => BinaryOperationType.Plus,
            "-" => BinaryOperationType.Minus,
            "*" => BinaryOperationType.Multy,
            ":" => BinaryOperationType.Divide,
            "!" => BinaryOperationType.Negate,
            "and" => BinaryOperationType.And,
            "or" => BinaryOperationType.Or,
            _ => BinaryOperationType.None
        };

    private static readonly Parser<char, BinaryOperationNode> BinaryOperation =
        Rec(
                () =>
                    from leftExp in OneOf(
                        UnaryOperation.Cast<ExpressionNode>(),
                        Call.Cast<ExpressionNode>(),
                        Expressionliteral.Cast<ExpressionNode>())
                    from op in Operator
                    from rightExp in Expression
                    select new BinaryOperationNode(
                        CreateOperatorType(op),
                        leftExp,
                        rightExp))
            .Labelled("BinaryExpression");

    private static readonly Parser<char, UnaryOperationNode> UnaryOperation =
        Rec(
                () =>
                    from op in Operator
                    from exp in Expression
                    select new UnaryOperationNode(
                        CreateOperatorType(op),
                        exp)
            )
            .Labelled("UnaryOperation");

    private static readonly Parser<char, ExpressionLiteralNode> Expressionliteral =
    (
        from txt in LetterOrDigit.ManyString().Before(SkipWhitespaces) //LetterOrDigit.AtLeastOnceUntil(Lookahead(Not(LetterOrDigit))).Before(SkipWhitespaces)
        select new ExpressionLiteralNode(new string(txt.ToArray()))
    ).Labelled("Literal");

    private static readonly Parser<char, ExpressionNode> Expression =
        OneOf(
                Try(UnaryOperation.Cast<ExpressionNode>()),
                Try(BinaryOperation.Cast<ExpressionNode>()),
                Try(Call.Cast<ExpressionNode>()),
                Expressionliteral.Cast<ExpressionNode>()
            )
            .Labelled("Expression");

    
    // public static Result<char, ExpressionNode> ParseExpression(string input)
    // {
    //     return Expression.Parse(input);
    // }

    #endregion

    #region TextFragments

    public static readonly Parser<char, ImmutableArray<FragmentAttributeNode>> AttributeParameters =
        CommaSeparated(
            from txt in Text.Before(DoublePoint)
            from exp in Expression
            select new FragmentAttributeNode(txt, exp)
        ).Between(OpenParen, CloseParen);

    #endregion
}