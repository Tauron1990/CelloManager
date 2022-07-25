using System.Linq.Expressions;
using System.Text.RegularExpressions;
using NStack;
using RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;
using RaiseOfNewWorld.Engine.Data.TextProcessing.Parsing;
using Terminal.Gui;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Templates;

public sealed class TemplateMatchCompiler : TemplateMatcherVisitor<Expression>
{
    public TemplateMatchCompiler()
    {
        ParameterExpression = Expression.Parameter(typeof(View));
    }

    public ParameterExpression ParameterExpression { get; }

    public override Expression VisitAndMatcher(AndMatcherNode andMatcherNode)
        => Expression.And(
            Accept(andMatcherNode.Left),
            Accept(andMatcherNode.Right));

    public override Expression VisitNameMatch(NameMatchNode nameMatchNode)
        => Expression.Equal(
            Expression.Property(
                ParameterExpression,
                "Id"),
            Expression.Constant(ustring.Make(nameMatchNode.Name)));

    public override Expression VisitNot(NotMatcherNode notMatcherNode)
        => Expression.Not(Accept(notMatcherNode.MatcherNode));

    public override Expression VisitOrMatcher(OrMatcherNode orMatcherNode)
        => Expression.Or(Accept(orMatcherNode.Left), Accept(orMatcherNode.Right));

    public override Expression VisitRegexMatcher(RegexMatcherNode regexMatcherNode)
    {
        var regex = new Regex(
            regexMatcherNode.Regex,
            RegexOptions.Compiled);


        return Expression.Call(
            Expression.Constant(regex),
            nameof(regex.IsMatch),
            new[] { typeof(string) },
            Expression.Call(
                Expression.Property(
                    ParameterExpression,
                    "Id"),
                "ToString",
                null));
    }

    public override Expression VisitTypeMatcher(TypeMatcherNode typeMatcherNode)
    {
        var type = Type.GetType(typeMatcherNode.TypeName) ?? Type.GetType($"Terminal.Gui.{typeMatcherNode.TypeName}");

        if (type is null)
            throw new InvalidOperationException($"No Type of {typeMatcherNode.TypeName} for type matcher Found");

        return Expression.Equal(
            Expression.Call(
                ParameterExpression,
                "GetType",
                null),
            Expression.Constant(type));
    }
}