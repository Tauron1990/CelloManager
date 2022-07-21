using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using FastExpressionCompiler;
using RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;
using RaiseOfNewWorld.Engine.Data.TextProcessing.Parsing;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing.PrimitiveVisitor;

public sealed class DoubleVisitor : AttributeValueVisitor<double>
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

    public override double VisitCall(CallAttributeValue callAttributeValue)
        => callAttributeValue.Parameters.Count switch
        {
            1 when _singleMethod.ContainsKey(callAttributeValue.MethodName)
                => _singleMethod[callAttributeValue.MethodName]
                    .Value(Accept(callAttributeValue.Parameters[0])),

            2 when _method.ContainsKey(callAttributeValue.MethodName)
                => _method[callAttributeValue.MethodName]
                    .Value(
                        Accept(callAttributeValue.Parameters[0]),
                        Accept(callAttributeValue.Parameters[1])
                    ),

            2 when callAttributeValue.MethodName == "multiply"
                => Accept(callAttributeValue.Parameters[0]) *
                   Accept(callAttributeValue.Parameters[1]),


            2 when callAttributeValue.MethodName == "divide"
                => Accept(callAttributeValue.Parameters[0]) /
                   Accept(callAttributeValue.Parameters[1]),

            _ => throw new InvalidOperationException(
                $"Invalid Count of parameter for Double Math Operation: {callAttributeValue.Parameters.Count} -- Or Method does not exist {callAttributeValue.MethodName}")
        };

    public override double VisitExpression(ExpressionAttributeValue expressionAttributeValue)
        => expressionAttributeValue.OperatorType switch
        {
            OperatorType.Add => Accept(expressionAttributeValue.Left) + Accept(expressionAttributeValue.Right),
            OperatorType.Subtract => Accept(expressionAttributeValue.Left) - Accept(expressionAttributeValue.Right),
            _ => throw new InvalidOperationException("No Operator Type Provided")
        };

    public override double VisitText(TextAttributeValue textAttributeValue)
    {
        var str = ResolveTextAttribute(textAttributeValue);
        return str switch
        {
            "epsilon" => double.Epsilon,
            "e" => Math.E,
            "tau" => Math.Tau,
            "pi" => Math.PI,
            _ => double.Parse(str)
        };
    }
}