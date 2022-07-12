using RaiseOfNewWorld.Engine.Data.TextProcessing.Parsing;
using Terminal.Gui;

namespace RaiseOfNewWorld.Engine.Data.TextProcessingOld;

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
            => token.MethodName switch
            {
                "width" => Width(token.ResolveParameter(0)),
                "percent" => Percent(token.ResolveParameter(0, int.Parse), token.ResolveParameter(1, bool.Parse, false)),
                "height" => Height(token.ResolveParameter(0)),
                "fill" => Fill(token.ResolveParameter(0, int.Parse, 0)),
                "sized" => Sized(token.ResolveParameter(0, int.Parse)),
                _ => Sized(token.ResolveParameter(-1, int.Parse))
            };

    }

    public static ExpressionNode<Dim> Parse(string input)
        => new DimParser(input).Create();
    
    private DimFabricatorExpression(Func<ViewContext, Dim> creationFunc) : base(creationFunc)
    {
    }

    private static DimFabricatorExpression Width(string name)
        => new(c => Dim.Width(c.GetView(name)));
    
    private static DimFabricatorExpression Percent(float n, bool r)
        => new(_ => Dim.Percent(n, r));
    
    private static DimFabricatorExpression Height(string name)
        => new(c => Dim.Height(c.GetView(name)));
    
    private static DimFabricatorExpression Fill(int margin)
        => new(_ => Dim.Fill(margin));
    
    private static DimFabricatorExpression Sized(int n)
        => new(_ => Dim.Sized(n));
}