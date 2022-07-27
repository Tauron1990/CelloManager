using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using FastExpressionCompiler;
using TextFragmentLib2.TextProcessing.Ast;
using TextFragmentLib2.TextProcessing.Parsing;

namespace TextFragmentLib2.TextProcessing.PrimitiveVisitor;

public sealed class DoubleVisitor : ExpressionNodeVisitor<double>
{
    public static readonly DoubleVisitor Instance = new();

    private static readonly ImmutableDictionary<string, Lazy<Func<double, double>>> _singleMethod = GetSimgleMethods();

    private static readonly ImmutableDictionary<string, Lazy<Func<double, double, double>>> _method = GetMethods();

    private static ImmutableDictionary<string, Lazy<Func<double, double>>> GetSimgleMethods()
        => typeof(Math).GetMethods()
            .Where(
                m => m.ReturnType == typeof(double) && ValidateParameters(
                    m,
                    typeof(double),
                    typeof(double)))
            .Select(m => (m.Name, Lazy: new Lazy<Func<double, double>>(GetDelegateFactory<Func<double, double>>(m))))
            .ToImmutableDictionary(
                t => t.Name,
                t => t.Lazy);

    private static ImmutableDictionary<string, Lazy<Func<double, double, double>>> GetMethods()
        => typeof(Math).GetMethods()
            .Where(
                m => m.ReturnType == typeof(double) && ValidateParameters(
                    m,
                    typeof(double),
                    typeof(double),
                    typeof(double)))
            .Select(
                m => (m.Name,
                    Lazy: new Lazy<Func<double, double, double>>(GetDelegateFactory<Func<double, double, double>>(m))))
            .ToImmutableDictionary(
                t => t.Name,
                t => t.Lazy);

    private static bool ValidateParameters(MethodBase methodInfo, params Type[] parameterTypes)
    {
        var parms = methodInfo.GetParameters();
        if (parms.Length != parameterTypes.Length) return false;

        return !parms.Where((t, i) => t.ParameterType != parameterTypes[i]).Any();
    }

    private static Func<TDelegate> GetDelegateFactory<TDelegate>(MethodInfo methodInfo)
        where TDelegate : Delegate
        => () => Expression.Lambda<TDelegate>(
                Expression.Call(
                    null,
                    methodInfo))
            .CompileFast();

    public override double VisitCall(CallExpressionNode callExpressionNode)
        => callExpressionNode.Parameters.Count switch
        {
            1 when _singleMethod.ContainsKey(callExpressionNode.MethodName)
                => _singleMethod[callExpressionNode.MethodName]
                    .Value(Accept(callExpressionNode.Parameters[0])),

            2 when _method.ContainsKey(callExpressionNode.MethodName)
                => _method[callExpressionNode.MethodName]
                    .Value(
                        Accept(callExpressionNode.Parameters[0]),
                        Accept(callExpressionNode.Parameters[1])
                    ),

            2 when callExpressionNode.MethodName == "multiply"
                => Accept(callExpressionNode.Parameters[0]) *
                   Accept(callExpressionNode.Parameters[1]),


            2 when callExpressionNode.MethodName == "divide"
                => Accept(callExpressionNode.Parameters[0]) /
                   Accept(callExpressionNode.Parameters[1]),

            _ => throw new InvalidOperationException(
                $"Invalid Count of parameter for Double Math Operation: {callExpressionNode.Parameters.Count} -- Or Method does not exist {callExpressionNode.MethodName}")
        };

    public override double VisitExpression(BinaryExpressionNode binaryExpressionNode)
        => binaryExpressionNode.OperatorType switch
        {
            OperatorType.Add => Accept(binaryExpressionNode.Left) + Accept(binaryExpressionNode.Right),
            OperatorType.Subtract => Accept(binaryExpressionNode.Left) - Accept(binaryExpressionNode.Right),
            _ => throw new InvalidOperationException("No Operator Type Provided")
        };

    public override double VisitText(LiteralExpressionNode literalExpressionNode)
    {
        var str = ResolveTextAttribute(literalExpressionNode);
        return str switch
        {
            "epsilon" => double.Epsilon,
            "e" => Math.E,
            "tau" => Math.Tau,
            "pi" => Math.PI,
            _ => double.Parse(str)
        };
    }

    public static int EvaluateInt(ExpressionBaseNode value)
        => throw new NotImplementedException();
}