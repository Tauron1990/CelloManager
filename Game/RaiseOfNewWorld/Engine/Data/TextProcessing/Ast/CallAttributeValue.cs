using System.Collections.Immutable;
using System.Text;
using Terminal.Gui;

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

    public override AttributeValueNode Merge(AttributeValueNode node)
    {
        if (node is not CallAttributeValue call) return base.Merge(node);
        
        if (call.MethodName != MethodName) return call;

        for (var i = 0; i < call.Parameters.Count; i++)
        {
            if (i < Parameters.Count)
            {
                var old = Parameters[i];
                Parameters = Parameters.Replace(Parameters[i], old.Merge(call.Parameters[i]));
            }
            else
                Parameters = Parameters.Add(call.Parameters[i]);
        }

        return base.Merge(node);
    }

    public override TReturn Visit<TReturn>(AttributeValueVisitor<TReturn> visitor)
        => visitor.VisitCall(this);
}