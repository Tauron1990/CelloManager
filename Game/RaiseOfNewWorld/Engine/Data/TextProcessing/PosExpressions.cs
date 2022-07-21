using System.Collections.Immutable;
using RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;
using RaiseOfNewWorld.Engine.Data.TextProcessing.Parsing;
using RaiseOfNewWorld.Engine.Data.TextProcessing.PrimitiveVisitor;
using Terminal.Gui;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing;

public sealed class PosVisitor : AttributeValueVisitor<Pos>
{
    public static Pos Evaluate(ImmutableDictionary<string, View> views, AttributeValueNode value)
        => new PosVisitor(views).Accept(value);
    
    private readonly ImmutableDictionary<string, View> _views;

    public PosVisitor(ImmutableDictionary<string, View> views)
        => _views = views;

    private View Lookup(string name)
    {
        if (_views.TryGetValue(
                name,
                out var view))
            return view;

        throw new InvalidOperationException("No View Found");
    }

    public override Pos VisitCall(CallAttributeValue callAttributeValue)
    {
        return callAttributeValue.MethodName switch
        {
            "anchorend" => Pos.AnchorEnd(callAttributeValue.GetIntParameter(0)),
            "y" => Pos.Y(Lookup(callAttributeValue.GetStringParameter(0))),
            "x" => Pos.X(Lookup(callAttributeValue.GetStringParameter(0))),
            "top" => Pos.Top(Lookup(callAttributeValue.GetStringParameter(0))),
            "right" => Pos.Right(Lookup(callAttributeValue.GetStringParameter(0))),
            "percent" => Pos.Percent(callAttributeValue.GetFloatParameter(0)),
            "left" => Pos.Left(Lookup(callAttributeValue.GetStringParameter(0))),
            "center" => Pos.Center(),
            "bottom" => Pos.Bottom(Lookup(callAttributeValue.GetStringParameter(0))),
            "at" => Pos.At(callAttributeValue.GetIntParameter(0)),
            _ => Pos.At(callAttributeValue.GetIntParameter(-1))
        };
    }

    public override Pos VisitExpression(ExpressionAttributeValue expressionAttributeValue)
        => expressionAttributeValue.OperatorType switch
        {
            OperatorType.Add => Accept(expressionAttributeValue.Left) + Accept(expressionAttributeValue.Right),
            OperatorType.Subtract => Accept(expressionAttributeValue.Left) - Accept(expressionAttributeValue.Left),
            _ => throw new InvalidOperationException("No Operator for pos provided")
        };

    public override Pos VisitText(TextAttributeValue textAttributeValue)
        => int.Parse(textAttributeValue.Value);
}