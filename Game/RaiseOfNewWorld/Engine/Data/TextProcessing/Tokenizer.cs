namespace RaiseOfNewWorld.Engine.Data.TextProcessing;

public static class Tokenizer
{
    public static Token NextToken(in ReadOnlySpan<char> text)
    {
        if (text.IsEmpty)
            return Token.Eof();

        for (var i = 0; i < text.Length; i++)
        {
            switch (text[i])
            {
                case '{':
                    return i == 0 
                        ? Token.NewSingle(text, TokenType.OpenFragment)
                        : Token.NewText(text, i);
                case '}':
                    return i == 0
                        ? Token.NewSingle(text, TokenType.CloseFragment)
                        : Token.NewText(text, i);
                case '(':
                    return i == 0
                        ? Token.NewSingle(text, TokenType.OpenAttribute)
                        : Token.NewText(text, i);
                case ')':
                    return i == 0
                        ? Token.NewSingle(text, TokenType.CloseAttribute)
                        : Token.NewText(text, i);
                case ':':
                    return i == 0
                        ? Token.NewSingle(text, TokenType.AttributeValueSeperator)
                        : Token.NewText(text, i);
                case ',':
                    return i == 0
                        ? Token.NewSingle(text, TokenType.AttributeSeperator)
                        : Token.NewText(text, i); 
            }
        }

        return new Token(text, TokenType.Text);
    }
}