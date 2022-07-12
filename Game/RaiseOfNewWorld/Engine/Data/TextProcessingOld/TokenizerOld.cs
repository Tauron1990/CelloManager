using System.Collections.Immutable;
using RaiseOfNewWorld.Engine.Data.TextProcessing.Parsing;

namespace RaiseOfNewWorld.Engine.Data.TextProcessingOld;

public class TokenizerOld
{
    private readonly ImmutableArray<TextToken> _tokens;
    public int Pointer { get; private set; }

    public bool CanNext => _tokens.Length > Pointer;
    
    private TokenizerOld(ImmutableArray<TextToken> tokens) => _tokens = tokens;

    public TextToken Get() => _tokens[Pointer];
    
    public TextToken GetAndIncement()
    {
        var token = Get();
        Pointer++;
        return token;
    }

    public void Incremnt()
    {
        Pointer++;
    }

    public TextToken GetNext()
        => _tokens[Pointer + 1];

    public TextToken GetPrevorius()
        => _tokens[Pointer - 1];
    
    public static TokenizerOld Tokens(in ReadOnlySpan<char> text)
    {
        var input = text;
        var tokens = ImmutableArray<TextToken>.Empty;
        TextToken textToken;
        var position = 0;
        
        do
        {
            textToken = NextToken(input, position);
            input = input[textToken.Text.Length..].Trim();
            position = position + textToken.Text.Length;
            
            tokens = tokens.Add(textToken);
        } while (!textToken.IsEof);

        return new TokenizerOld(tokens);
    }

    private static TextToken NextToken(in ReadOnlySpan<char> text, int position)
    {
        if (text.IsEmpty)
            return TextToken.Eof;

        for (var i = 0; i < text.Length; i++)
        {
            switch (text[i])
            {
                case '-':
                    return TextToken.GetToken(text, i, TokenType.Minus, position);
                case '+':
                    return TextToken.GetToken(text, i, TokenType.Plus, position);
                case '$':
                    return TextToken.GetToken(text, i, TokenType.Template, position);
                case '{':
                    return TextToken.GetToken(text, i, TokenType.OpenFragment, position);
                case '}':
                    return TextToken.GetToken(text, i, TokenType.CloseFragment, position);
                case '(':
                    return TextToken.GetToken(text, i, TokenType.OpenAttribute, position);
                case ')':
                    return TextToken.GetToken(text, i, TokenType.CloseAttribute, position);
                case ':':
                    return TextToken.GetToken(text, i, TokenType.AttributeValueSeperator, position);
                case ',':
                    return TextToken.GetToken(text, i, TokenType.AttributeSeperator, position);
            }
        }

        return new TextToken(text.ToString(), TokenType.Text, position);
    }

    public void Decrement() => Pointer--;
}