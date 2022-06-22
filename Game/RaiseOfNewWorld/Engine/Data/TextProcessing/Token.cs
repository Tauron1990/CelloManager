namespace RaiseOfNewWorld.Engine.Data.TextProcessing;

public enum TokenType
{
    Eof,
    Text,
    OpenFragment,
    CloseFragment,
    OpenAttribute,
    CloseAttribute,
    AttributeValueSeperator,
    AttributeSeperator
}

public readonly ref struct Token
{
    public static Token Eof() => new(ReadOnlySpan<char>.Empty, TokenType.Eof);

    public readonly ReadOnlySpan<char> Text;
    public readonly TokenType TokenType;
    
    public bool IsEof => TokenType == TokenType.Eof;

    public Token(ReadOnlySpan<char> text, TokenType tokenType)
    {
        Text = text;
        TokenType = tokenType;
    }

    public static Token NewSingle(in ReadOnlySpan<char> text, TokenType type)
        => new(text[..1], type);
    
    public static Token NewText(in ReadOnlySpan<char> text, int index)
        => new(text[..(index - 1)], TokenType.Text);

    public override string ToString()
        => $"{Text} -- {TokenType}";
}