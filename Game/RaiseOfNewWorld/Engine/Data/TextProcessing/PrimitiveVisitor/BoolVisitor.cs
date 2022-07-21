﻿using RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;
using RaiseOfNewWorld.Engine.Data.TextProcessing.Parsing;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing.PrimitiveVisitor;

public sealed class BoolVisitor : AttributeValueVisitor<bool>
{
    public static readonly BoolVisitor Instance = new();

    public override bool VisitCall(CallAttributeValue callAttributeValue)
        => callAttributeValue.MethodName switch
        {
            "not" => callAttributeValue.Parameters.Select(Accept).All(b => !b),
            _ => callAttributeValue.Parameters.Select(Accept).All(b => b)
        };

    public override bool VisitExpression(ExpressionAttributeValue expressionAttributeValue)
    {
        var valueLeft = Accept(expressionAttributeValue.Left);
        var valueRight = Accept(expressionAttributeValue.Right);

        return expressionAttributeValue.OperatorType switch
        {
            OperatorType.Add => valueLeft && valueRight,
            OperatorType.Subtract => valueLeft || valueRight,
            _ => throw new InvalidOperationException("Invalid operation type for Boolean")
        };
    }

    public override bool VisitText(TextAttributeValue textAttributeValue)
        => throw new NotImplementedException();
}