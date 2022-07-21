using RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing.PrimitiveVisitor;

public static class AttributeNodeExtensions
{
    public static string GetStringParameter(this CallAttributeValue callAttributeValue, int pos)
        => pos == -1
            ? callAttributeValue.MethodName
            : StringVisitor.Instance.ToString(callAttributeValue.Parameters[pos]);

    public static int GetIntParameter(this CallAttributeValue callAttributeValue, int pos)
        => pos == -1
            ? int.Parse(callAttributeValue.MethodName)
            : (int)DoubleVisitor.Instance.Accept(callAttributeValue.Parameters[pos]);

    public static float GetFloatParameter(this CallAttributeValue callAttributeValue, int pos)
        => pos == -1
            ? float.Parse(callAttributeValue.MethodName)
            : (float)DoubleVisitor.Instance.Accept(callAttributeValue.Parameters[pos]);

    public static bool GetBoolParameter(this CallAttributeValue callAttributeValue, int pos)
        => pos == -1
            ? bool.Parse(callAttributeValue.MethodName)
            : BoolVisitor.Instance.Accept(callAttributeValue.Parameters[pos]);
}