namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

public sealed class TypeMatcherNode : TemplateMatcherNode
{
    public string TypeName { get; set; } = string.Empty;
    

    protected override string Format()
        => TypeName;
}