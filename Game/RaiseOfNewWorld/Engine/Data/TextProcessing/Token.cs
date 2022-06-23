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

public readonly struct Token
{
    public static readonly Token Eof = new(string.Empty, TokenType.Eof);

    public readonly string Text;
    public readonly TokenType TokenType;
    
    public bool IsEof => TokenType == TokenType.Eof;

    public Token(string text, TokenType tokenType)
    {
        Text = text;
        TokenType = tokenType;
    }

    public static Token NewSingle(in ReadOnlySpan<char> text, TokenType type)
        => new(text[..1].ToString(), type);
    
    public static Token NewText(in ReadOnlySpan<char> text, int index)
        => new(text[..index].ToString(), TokenType.Text);

    public override string ToString()
        => $"{Text} -- {TokenType}";
}