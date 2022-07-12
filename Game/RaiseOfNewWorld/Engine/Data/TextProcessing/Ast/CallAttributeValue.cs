using System.Collections.Immutable;
using System.Text;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

public sealed class CallAttributeValue : AttributeValueNode
{
    public string MethodName { get; set; } = string.Empty;
    
    public ImmutableList<AttributeValueNode> Parameters { get; set; } = ImmutableList<AttributeValueNode>.Empty;
    protected override string Format()
    {
        return new StringBuilder()
            .Append(MethodName)
            .Append('(')
            .AppendJoin(',', Parameters)
            .Append(')')
            .ToString();
    }
}