using System.Collections.Immutable;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing;

public class Tokenizer
{
    private readonly ImmutableArray<Token> _tokens;
    public int Pointer { get; private set; }

    public bool CanNext => _tokens.Length > Pointer;
    
    private Tokenizer(ImmutableArray<Token> tokens) => _tokens = tokens;

    public Token Get() => _tokens[Pointer];
    
    public Token GetAndIncement()
    {
        var token = Get();
        Pointer++;
        return token;
    }

    public void Incremnt()
    {
        Pointer++;
    }

    public Token GetNext()
        => _tokens[Pointer + 1];

    public Token GetPrevorius()
        => _tokens[Pointer - 1];
    
    public static Tokenizer Tokens(in ReadOnlySpan<char> text)
    {
        var input = text;
        var tokens = ImmutableArray<Token>.Empty;
        Token token;

        do
        {
            token = NextToken(input);
            input = input[token.Text.Length..];

            tokens = tokens.Add(token);
        } while (!token.IsEof);

        return new Tokenizer(tokens);
    }

    private static Token NextToken(in ReadOnlySpan<char> text)
    {
        if (text.IsEmpty)
            return Token.Eof;

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

        return new Token(text.ToString(), TokenType.Text);
    }
}

public class StringTokenizer
{
    private readonly ImmutableArray<string> _tokens;
    public int Pointer { get; private set; }

    public bool CanNext => _tokens.Length > Pointer;
    
    public StringTokenizer(ImmutableArray<string> tokens) => _tokens = tokens;

    public string Get() => _tokens[Pointer];
    
    public string GetAndIncement()
    {
        var token = Get();
        Pointer++;
        return token;
    }

    public void Incremnt()
    {
        Pointer++;
    }

    public string GetNext()
        => _tokens[Pointer + 1];

    public string GetPrevorius()
        => _tokens[Pointer - 1];
}