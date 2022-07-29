namespace TextFragmentLib2.TextProcessing.Ast;

public sealed class TypeRepesentation : AstNode
{
    public static readonly TypeRepesentation Empty = new(string.Empty, string.Empty);

    public string Type { get; }

    public string Parameter { get; }

    public TypeRepesentation(string type, string parameter)
    {
        Type = type;
        Parameter = parameter;
    }

    public override void Validate()
    {
    }

    protected override string Format()
    {
        if (string.IsNullOrWhiteSpace(Parameter))
            return Type;
        return $"{Type}-{Parameter}";
    }
}