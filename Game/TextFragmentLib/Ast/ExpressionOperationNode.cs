﻿namespace TextFragmentLib.Ast;

public abstract class ExpressionOperationNode : ExpressionNode
{
    public ExpressionOperationNode(BinaryOperationType operationType)
    {
        OperationType = operationType;
    }

    public BinaryOperationType OperationType { get; }

    public override void Validate()
    {
        if (OperationType == BinaryOperationType.None)
            ThrowValidationError("OperationType none is invalid");
    }

    protected string FormatOperationType()
        => OperationType switch
        {
            BinaryOperationType.Plus => "+",
            BinaryOperationType.Minus => "-",
            BinaryOperationType.Multy => "*",
            BinaryOperationType.Divide => ":",
            BinaryOperationType.Negate => "!",
            BinaryOperationType.And => "and",
            BinaryOperationType.Or => "or",
            _ => string.Empty
        };
}