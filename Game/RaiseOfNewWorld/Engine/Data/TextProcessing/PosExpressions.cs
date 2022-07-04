using RaiseOfNewWorld.Engine.Rooms;
using Terminal.Gui;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing;

public sealed class PosAddExpressions : AddExpression<Pos>
{
    public PosAddExpressions() : base((_, r, l) => r + l)
    {
    }
}

public sealed class PosSubstractExpression : SubstractExpression<Pos>
{
    public PosSubstractExpression() : base((_, r, l) => r - l)
    {
    }
}

public sealed class PosFabricatorExpression : FabricatorExpression<Pos>
{
    public PosFabricatorExpression(Func<ViewContext, FabricatorData, Pos> creationFunc) : base(creationFunc)
    {
    }
}
