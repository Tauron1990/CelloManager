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
    private class PosParser : ExpressionParser
    {
        public PosParser(string input) : base(input)
        {
        }

        protected override SubstractExpression<Pos> CreateSubstract(ExpressionNode<Pos> first, StringTokenizer input)
            => new PosSubstractExpression { Right = first, Left = Create(input) };

        protected override AddExpression<Pos> CreateAdd(ExpressionNode<Pos> first, StringTokenizer input)
            => new PosAddExpressions { Right = first, Left = Create(input) };

        protected override ExpressionNode<Pos> CreateFabricator(ParameterParser token) =>
            token.MethodName switch
            {
                "anchorend" => AnchorEnd(token.CanResolve(0) ? token.ResolveParameter(0, int.Parse) : 0),
                "y" => Y(token.ResolveParameter(0)),
                "x" => X(token.ResolveParameter(0)),
                "top" => Top(token.ResolveParameter(0)),
                "right" => Right(token.ResolveParameter(0)),
                "percent" => Percent(token.ResolveParameter(0, float.Parse)),
                "left" => Left(token.ResolveParameter(0)),
                "center" => Center(),
                "bottom" => Bottom(token.ResolveParameter(0)),
                "at" => At(token.ResolveParameter(0, int.Parse)),
                _ => At(token.ResolveParameter(-1, int.Parse))
            };
    }

    private PosFabricatorExpression(Func<ViewContext, Pos> creationFunc) : base(creationFunc)
    {
    }

    public static ExpressionNode<Pos> Parse(string input)
    {
        var parser = new PosParser(input);
        return parser.Create();
    }

    private static PosFabricatorExpression AnchorEnd(int margin)
        => new(_ => Pos.AnchorEnd(margin));
    
    private static PosFabricatorExpression Y(string name)
        => new(c => Pos.Y(c.GetView(name)));
    
    private static PosFabricatorExpression X(string name)
        => new(c => Pos.X(c.GetView(name)));
    
    private static PosFabricatorExpression Top(string name)
        => new(c => Pos.Top(c.GetView(name)));
    
    private static PosFabricatorExpression Right(string name)
        => new(c => Pos.Right(c.GetView(name)));
    
    private static PosFabricatorExpression Percent(float input)
        => new(_ => Pos.Percent(input));
    
    private static PosFabricatorExpression At(int input) 
        => new(_ => Pos.At(input));

    private static PosFabricatorExpression Bottom(string viewName)
        => new(c => Pos.Bottom(c.GetView(viewName)));

    private static PosFabricatorExpression Center()
        => new(_ => Pos.Center());

    private static PosFabricatorExpression Left(string viewName)
        => new(c => Pos.Left(c.GetView(viewName)));
}
