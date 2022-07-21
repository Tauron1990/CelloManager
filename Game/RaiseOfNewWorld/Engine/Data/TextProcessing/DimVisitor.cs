using System.Collections.Immutable;
using RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;
using RaiseOfNewWorld.Engine.Data.TextProcessing.Parsing;
using RaiseOfNewWorld.Engine.Data.TextProcessing.PrimitiveVisitor;
using Terminal.Gui;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing;

public sealed class DimVisitor : AttributeValueVisitor<Dim>
{
    public static Dim Evaluate(ImmutableDictionary<string, View> views, AttributeValueNode value)
        => new DimVisitor(views).Accept(value);
    
    private readonly ImmutableDictionary<string, View> _views;

    public DimVisitor(ImmutableDictionary<string, View> views)
    {
        _views = views;
    }

    private View Lookup(string name)
    {
        if (_views.TryGetValue(
                name,
                out var view))
            return view;

        throw new InvalidOperationException("No View Found");
    }

    public override Dim VisitCall(CallAttributeValue callAttributeValue)
        => callAttributeValue.MethodName switch
        {
            "width" => Dim.Width(Lookup(callAttributeValue.GetStringParameter(0))),
            "percent" => Dim.Percent(
                callAttributeValue.GetIntParameter(0),
                callAttributeValue.GetBoolParameter(1)),
            "height" => Dim.Height(Lookup(callAttributeValue.GetStringParameter(0))),
            "fill" => Dim.Fill(callAttributeValue.GetIntParameter(0)),
            "sized" => Dim.Sized(callAttributeValue.GetIntParameter(0)),
            _ => Dim.Sized(callAttributeValue.GetIntParameter(-1))
        };

    public override Dim VisitExpression(ExpressionAttributeValue expressionAttributeValue)
        => expressionAttributeValue.OperatorType switch
        {
            OperatorType.Add => Accept(expressionAttributeValue.Left) + Accept(expressionAttributeValue.Right),
            OperatorType.Subtract => Accept(expressionAttributeValue.Left) - Accept(expressionAttributeValue.Right),
            _ => throw new InvalidOperationException("No Operator Setted")
        };

    public override Dim VisitText(TextAttributeValue textAttributeValue) => int.Parse(textAttributeValue.Value);
}