using TextFragmentLib2.TextProcessing.Ast;

namespace TextFragmentLib2.TextProcessing.PrimitiveVisitor;

public static class ExpressionNodeExtensions
{
    public static string GetStringParameter(this CallExpressionNode callExpressionNode, int pos)
        => pos == -1
            ? callExpressionNode.MethodName
            : StringVisitor.Instance.ToString(callExpressionNode.Parameters[pos]);

    public static int GetIntParameter(this CallExpressionNode callExpressionNode, int pos)
        => pos == -1
            ? int.Parse(callExpressionNode.MethodName)
            : (int)DoubleVisitor.Instance.Accept(callExpressionNode.Parameters[pos]);

    public static float GetFloatParameter(this CallExpressionNode callExpressionNode, int pos)
        => pos == -1
            ? float.Parse(callExpressionNode.MethodName)
            : (float)DoubleVisitor.Instance.Accept(callExpressionNode.Parameters[pos]);

    public static bool GetBoolParameter(this CallExpressionNode callExpressionNode, int pos)
        => pos == -1
            ? bool.Parse(callExpressionNode.MethodName)
            : BoolVisitor.Instance.Accept(callExpressionNode.Parameters[pos]);
}