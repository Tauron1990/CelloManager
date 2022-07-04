using Terminal.Gui;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing;

public sealed class DimAddExpressions : AddExpression<Dim>
{
    public DimAddExpressions() : base((_, r, l) => r + l)
    {
    }
}

public sealed class DimSubstractExpression : SubstractExpression<Dim>
{
    public DimSubstractExpression() : base((_, r, l) => r - l)
    {
    }
}

public sealed class DimFabricatorExpression : FabricatorExpression<Dim>
{
    public DimFabricatorExpression(Func<ViewContext, FabricatorData, Dim> creationFunc) : base(creationFunc)
    {
    }
}