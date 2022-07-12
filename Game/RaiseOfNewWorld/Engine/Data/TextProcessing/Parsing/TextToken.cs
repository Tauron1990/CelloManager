using System.Buffers;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Parsing;

public enum TokenType
{
    Eof,
    Text,
    Template,
    OpenFragment,
    CloseFragment,
    OpenAttribute,
    CloseAttribute,
    AttributeValueSeperator,
    AttributeSeperator,
    Plus,
    Minus
}

public readonly struct TextToken
{
    public static readonly TextToken Eof = new(string.Empty, TokenType.Eof, -1);

    public readonly string Text;
    public readonly TokenType TokenType;
    public readonly int Position;

    public bool IsEof => TokenType == TokenType.Eof;

    public TextToken(string text, TokenType tokenType, int position)
    {
        Text = text;
        TokenType = tokenType;
        Position = position;
    }

    private static string MakeString(in ReadOnlySpan<char> text)
    {
        var arr = ArrayPool<char>.Shared.Rent(text.Length);
        try
        {
            var output = arr.AsSpan();
            var count = text.Trim().ToLower(output, null);

            return output[..count].ToString();
        }
        finally
        {
            ArrayPool<char>.Shared.Return(arr);
        }
    }
    
    private static TextToken NewSingle(in ReadOnlySpan<char> text, TokenType type, int position)
        => new(MakeString(text[..1]), type, position);

    private static TextToken NewText(in ReadOnlySpan<char> text, int index, int position)
        => new(MakeString(text[..index]), TokenType.Text, position);

    public static TextToken GetToken(in ReadOnlySpan<char> text, int index, TokenType type, int position)
        => index == 0
            ? NewSingle(text, type, position)
            : NewText(text, index, position);
    
    public override string ToString()
        => $"{Text} -- {TokenType} -- Position:{Position}";
}