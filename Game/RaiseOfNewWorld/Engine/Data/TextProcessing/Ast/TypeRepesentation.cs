namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

public sealed class TypeRepesentation : AstNode
{
    public static readonly TypeRepesentation Empty = new();
    
    public string Type { get; set; } = string.Empty;

    public string Parameter { get; set; } = string.Empty;
    
    public override void Validate(){ }

    protected override string Format()
    {
        if (string.IsNullOrWhiteSpace(Parameter))
            return Type;
        return $"{Type}-{Parameter}";
    }
}