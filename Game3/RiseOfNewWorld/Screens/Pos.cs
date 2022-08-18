using JetBrains.Annotations;

namespace RiseOfNewWorld.Screens;

[PublicAPI]
public class Pos
{
    private static PosAnchorEnd? _endNoMargin;

    private static PosCenter? _pCenter;

    private static PosCombine? _posCombine;

    internal virtual float Anchor(int width) => 0;

    public static Pos Function(Func<int> function) => new PosFunc(function);

    public static Pos Percent(float n)
    {
        if(n is < 0 or > 100)
            throw new ArgumentException("Percent value must be between 0 and 100");

        return new PosFactor(n / 100);
    }

    public static Pos AnchorEnd(int margin = 0) =>
        margin switch
        {
            < 0 => throw new ArgumentException("Margin must be positive"),
            0 => _endNoMargin ??= new PosAnchorEnd(0),
            _ => new PosAnchorEnd(margin)
        };


    public static Pos Center() => _pCenter ??= new PosCenter();

    public static implicit operator Pos(int n) => new PosAbsolute(n);

    public static Pos At(int n) => new PosAbsolute(n);

    public static Pos operator +(Pos left, Pos right)
    {
        if(left is PosAbsolute && right is PosAbsolute)
        {
            _posCombine = null;
            return new PosAbsolute(left.Anchor(0) + right.Anchor(0));
        }

        var newPos = new PosCombine(true, left, right);
        SetPosCombine(left, newPos);
        return _posCombine = newPos;
    }

    public static Pos operator -(Pos left, Pos right)
    {
        if(left is PosAbsolute && right is PosAbsolute)
        {
            _posCombine = null;
            return new PosAbsolute(left.Anchor(0) - right.Anchor(0));
        }

        var newPos = new PosCombine(false, left, right);
        SetPosCombine(left, newPos);
        return _posCombine = newPos;
    }

    private static void SetPosCombine(Pos left, PosCombine? newPos)
    {
        // if(_posCombine?.ToString() != newPos?.ToString())
        // {
        //     if(left is PosView view)
        //         view.Target.SetNeedsLayout();
        // }
    }


    public static Pos Left(RenderElement view) => new PosCombine(true, new PosView(view, 0), new PosAbsolute(0));


    public static Pos X(RenderElement view) => new PosCombine(true, new PosView(view, 0), new PosAbsolute(0));


    public static Pos Top(RenderElement view) => new PosCombine(true, new PosView(view, 1), new PosAbsolute(0));


    public static Pos Y(RenderElement view) => new PosCombine(true, new PosView(view, 1), new PosAbsolute(0));

    public static Pos Right(RenderElement view) => new PosCombine(true, new PosView(view, 2), new PosAbsolute(0));


    public static Pos Bottom(RenderElement view) => new PosCombine(true, new PosView(view, 3), new PosAbsolute(0));

    public override int GetHashCode() => Anchor(0).GetHashCode();

    public override bool Equals(object? other) => other is Pos abs && abs == this;

    // Helper class to provide dynamic value by the execution of a function that returns an integer.
    internal class PosFunc : Pos
    {
        private readonly Func<int> _function;

        public PosFunc(Func<int> n) => _function = n;

        internal override float Anchor(int width) => _function();

        public override string ToString() => $"Pos.PosFunc({_function()})";

        public override int GetHashCode() => _function.GetHashCode();

        public override bool Equals(object? other) => other is PosFunc f && f._function() == _function();
    }

    internal class PosFactor : Pos
    {
        private readonly float _factor;

        public PosFactor(float n) => _factor = n;

        internal override float Anchor(int width) => (int)(width * _factor);

        public override string ToString() => $"Pos.Factor({_factor})";

        public override int GetHashCode() => _factor.GetHashCode();

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public override bool Equals(object? other) => other is PosFactor f && f._factor == _factor;
    }

    internal class PosAnchorEnd : Pos
    {
        private readonly int _n;

        public PosAnchorEnd(int n) => _n = n;

        internal override float Anchor(int width) => width - _n;

        public override string ToString() => $"Pos.AnchorEnd(margin={_n})";
    }

    internal class PosCenter : Pos
    {
        internal override float Anchor(int width) => width / 2f;

        public override string ToString() => "Pos.Center";
    }

    internal class PosAbsolute : Pos
    {
        private readonly float _n;

        public PosAbsolute(float n) => _n = n;

        public override string ToString() => $"Pos.Absolute({_n})";

        internal override float Anchor(int width) => _n;

        public override int GetHashCode() => _n.GetHashCode();

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public override bool Equals(object? other) => other is PosAbsolute abs && abs._n == _n;
    }

    internal class PosCombine : Pos
    {
        private readonly bool _add;
        private readonly Pos _left;
        private readonly Pos _right;

        public PosCombine(bool add, Pos left, Pos right)
        {
            _left = left;
            _right = right;
            _add = add;
        }

        internal override float Anchor(int width)
        {
            var la = _left.Anchor(width);
            var ra = _right.Anchor(width);
            if(_add)
                return la + ra;

            return la - ra;
        }

        public override string ToString() => $"Pos.Combine({_left.ToString()}{(_add ? '+' : '-')}{_right.ToString()})";
    }

    internal class PosView : Pos
    {
        private readonly int _side;
        private readonly RenderElement _target;

        public PosView(RenderElement view, int side)
        {
            _target = view;
            _side = side;
        }

        internal override float Anchor(int width)
        {
            switch (_side)
            {
                case 0: return _target.AbsolutPosition.X;
                case 1: return _target.AbsolutPosition.Y;
                case 2: return _target.AbsolutPosition.width;
                case 3: return _target.AbsolutPosition.height;
                default:
                    return 0;
            }
        }

        public override string ToString()
        {
            var tside = _side switch
            {
                0 => "x",
                1 => "y",
                2 => "right",
                3 => "bottom",
                _ => "unknown"
            };

            return $"Pos.View(side={tside}, target={_target.ToString()})";
        }

        public override int GetHashCode() => _target.GetHashCode();

        public override bool Equals(object? other) => other is PosView abs && abs._target == _target;
    }
}

[PublicAPI]
public class Dim
{
    private static DimFill? _zeroMargin;

    private static DimCombine? _dimCombine;

    internal virtual float Anchor(int width) => 0;


    public static Dim Function(Func<int> function) => new DimFunc(function);

    public static Dim Percent(float n, bool r = false)
    {
        if(n is < 0 or > 100)
            throw new ArgumentException("Percent value must be between 0 and 100");

        return new DimFactor(n / 100, r);
    }

    public static Dim Fill(int margin = 0)
    {
        if(margin == 0)
            return _zeroMargin ??= new DimFill(0);

        return new DimFill(margin);
    }

    public static implicit operator Dim(int n) => new DimAbsolute(n);

    public static Dim Sized(int n) => new DimAbsolute(n);

    public static Dim operator +(Dim left, Dim right)
    {
        if(left is DimAbsolute && right is DimAbsolute)
        {
            _dimCombine = null;
            return new DimAbsolute(left.Anchor(0) + right.Anchor(0));
        }

        var newDim = new DimCombine(true, left, right);
        SetDimCombine(left, newDim);
        return _dimCombine = newDim;
    }

    public static Dim operator -(Dim left, Dim right)
    {
        if(left is DimAbsolute && right is DimAbsolute)
        {
            _dimCombine = null;
            return new DimAbsolute(left.Anchor(0) - right.Anchor(0));
        }

        var newDim = new DimCombine(false, left, right);
        SetDimCombine(left, newDim);
        return _dimCombine = newDim;
    }

    private static void SetDimCombine(Dim left, DimCombine? newPos)
    {
        //if(_dimCombine?.ToString() != newPos?.ToString())
        // {
        //     if(left is DimView view)
        //         view.Target.SetNeedsLayout();
        // }
    }

    public static Dim Width(RenderElement view) => new DimView(view, 1);

    public static Dim Height(RenderElement view) => new DimView(view, 0);

    public override int GetHashCode() => Anchor(0).GetHashCode();

    public override bool Equals(object? other) => other is Dim abs && abs == this;

    // Helper class to provide dynamic value by the execution of a function that returns an integer.
    internal class DimFunc : Dim
    {
        private readonly Func<int> _function;

        public DimFunc(Func<int> n) => _function = n;

        internal override float Anchor(int width) => _function();

        public override string ToString() => $"Dim.DimFunc({_function()})";

        public override int GetHashCode() => _function.GetHashCode();

        public override bool Equals(object? other) => other is DimFunc f && f._function() == _function();
    }

    internal class DimFactor : Dim
    {
        private readonly float _factor;
        private readonly bool _remaining;

        public DimFactor(float n, bool r = false)
        {
            _factor = n;
            _remaining = r;
        }

        internal override float Anchor(int width) => (int)(width * _factor);

        //public bool IsFromRemaining() => _remaining;

        public override string ToString() => $"Dim.Factor(factor={_factor}, remaining={_remaining})";

        public override int GetHashCode() => _factor.GetHashCode();

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public override bool Equals(object? other) => other is DimFactor f && f._factor == _factor && f._remaining == _remaining;
    }

    internal class DimAbsolute : Dim
    {
        private readonly float _n;

        public DimAbsolute(float n) => _n = n;

        public override string ToString() => $"Dim.Absolute({_n})";

        internal override float Anchor(int width) => _n;

        public override int GetHashCode() => _n.GetHashCode();

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public override bool Equals(object? other) => other is DimAbsolute abs && abs._n == _n;
    }

    internal class DimFill : Dim
    {
        private readonly int _margin;

        public DimFill(int margin) => _margin = margin;

        public override string ToString() => $"Dim.Fill(margin={_margin})";

        internal override float Anchor(int width) => width - _margin;

        public override int GetHashCode() => _margin.GetHashCode();

        public override bool Equals(object? other) => other is DimFill fill && fill._margin == _margin;
    }

    internal class DimCombine : Dim
    {
        private readonly bool _add;
        private readonly Dim _left;
        private readonly Dim _right;

        public DimCombine(bool add, Dim left, Dim right)
        {
            _left = left;
            _right = right;
            _add = add;
        }

        internal override float Anchor(int width)
        {
            var la = _left.Anchor(width);
            var ra = _right.Anchor(width);
            if(_add)
                return la + ra;

            return la - ra;
        }

        public override string ToString() => $"Dim.Combine({_left.ToString()}{(_add ? '+' : '-')}{_right.ToString()})";
    }

    internal class DimView : Dim
    {
        private readonly int _side;
        private readonly RenderElement _target;

        public DimView(RenderElement view, int side)
        {
            _target = view;
            _side = side;
        }

        internal override float Anchor(int width)
        {
            switch (_side)
            {
                case 0: return _target.AbsolutPosition.height;
                case 1: return _target.AbsolutPosition.width;
                default:
                    return 0;
            }
        }

        public override string ToString()
        {
            string tside;
            switch (_side)
            {
                case 0:
                    tside = "Height";
                    break;
                case 1:
                    tside = "Width";
                    break;
                default:
                    tside = "unknown";
                    break;
            }

            return $"DimView(side={tside}, target={_target.ToString()})";
        }

        public override int GetHashCode() => _target.GetHashCode();

        public override bool Equals(object? other) => other is DimView abs && abs._target == _target;
    }
}