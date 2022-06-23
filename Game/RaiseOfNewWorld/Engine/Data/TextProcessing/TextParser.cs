using System.Collections.Immutable;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing;

public static class TextParser
{
    public static IEnumerable<ITextData> ParseText(string text)
    {
        var tokenizer = Tokenizer.Tokens(text);

        while ( tokenizer.CanNext && tokenizer.Get().TokenType is not TokenType.Eof)
            yield return ReadFragment(tokenizer);
    }

    private static ITextData ReadFragment(Tokenizer tokens)
    {
        var token = tokens.Get();
        if(token.TokenType is TokenType.OpenFragment)
            tokens.Incremnt();
        
        var startToken = tokens.GetAndIncement();
        if (startToken.IsEof)
            throw new InvalidOperationException("Unexpected End of File");

        if (startToken.TokenType is not TokenType.Text)
            throw new InvalidOperationException("A Fragment Should start with Text");

        var nextToken = tokens.Get();

        switch (nextToken)
        {
            case { TokenType: TokenType.CloseFragment }:
                tokens.Incremnt();
                return new SimpleText(startToken.Text);
            case { TokenType: TokenType.AttributeValueSeperator or TokenType.OpenAttribute }:
                var basicData = ReadMetadata(startToken.Text, tokens);
                ReadFragmentContent(ref basicData, tokens);
                return basicData;
            case { TokenType: TokenType.OpenFragment or TokenType.Eof }:
                tokens.Incremnt();
                return new SimpleText(startToken.Text);
            default:
                ThrowInvalidToken(startToken, tokens);
                return null!;
        }
    }

    private static TextData ReadMetadata(string name, Tokenizer tokens)
    {
        var attributes = ImmutableArray<AttributeData>.Empty;
        string? type = null;

        if (tokens.Get().TokenType is TokenType.AttributeValueSeperator)
        {
            tokens.Incremnt();
            type = tokens.GetAndIncement().Text;
        }

        ValidateToken(tokens, tokens.GetAndIncement(), TokenType.OpenAttribute);


        while (tokens.Get().TokenType is not TokenType.CloseAttribute or TokenType.Eof)
        {
            var attrName = ValidateToken(tokens, tokens.GetAndIncement(), TokenType.Text);
            ValidateToken(tokens, tokens.GetAndIncement(), TokenType.AttributeValueSeperator);
            var attrValue = ValidateToken(tokens, tokens.GetAndIncement(), TokenType.Text);

            attributes = attributes.Add(new AttributeData(attrName.Text, attrValue.Text));

            var token = tokens.GetAndIncement();
            if(token.TokenType is TokenType.CloseAttribute)
                break;
            if(token.TokenType is not TokenType.AttributeSeperator)
                ThrowInvalidToken(token, tokens);
        }
        
        if(tokens.Get().TokenType is TokenType.CloseAttribute)
            tokens.Incremnt();

        return new TextData(name, type, attributes, ImmutableArray<ITextData>.Empty);
    }

    private static void ReadFragmentContent(ref TextData data, Tokenizer tokens)
    {
        var textData = ImmutableArray<ITextData>.Empty;

        while (tokens.Get().TokenType is not TokenType.CloseFragment or TokenType.Eof)
        {
            var token = tokens.GetAndIncement();
            switch (token.TokenType)
            {
                case TokenType.Text:
                    textData = textData.Add(new SimpleText(token.Text));
                    break;
                case TokenType.OpenFragment:
                    textData = textData.Add(ReadFragment(tokens));
                    break;
                default:
                    ThrowInvalidToken(token, tokens);
                    break;
            }
        }

        ValidateToken(tokens, tokens.GetAndIncement(), TokenType.CloseFragment);
        data = data with { Content = textData };
    }

    private static Token ValidateToken(Tokenizer tokens, Token token, TokenType expected)
    {
        if(token.TokenType != expected)
            ThrowInvalidToken(token, tokens);

        return token;
    }
    
    private static void ThrowInvalidToken(Token token, Tokenizer tokenArray)
    {
        if(tokenArray.Pointer > 0)
            throw new InvalidOperationException($"Invalid Token: {token} after {tokenArray.GetPrevorius()}");
        throw new InvalidOperationException($"Invalid Token: {token}");
    }
}