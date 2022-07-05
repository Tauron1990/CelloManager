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
    private sealed class DimParser : ExpressionParser
    {
        public DimParser(string input) : base(input)
        {
        }

        protected override SubstractExpression<Dim> CreateSubstract(ExpressionNode<Dim> first, StringTokenizer input)
            => new DimSubstractExpression { Right = first, Left = Create(input) };

        protected override AddExpression<Dim> CreateAdd(ExpressionNode<Dim> first, StringTokenizer input)
            => new DimAddExpressions { Right = first, Left = Create(input) };

        protected override ExpressionNode<Dim> CreateFabricator(ParameterParser token)
            
    }
    
    public DimFabricatorExpression(Func<ViewContext, Dim> creationFunc) : base(creationFunc)
    {
        
    }
}