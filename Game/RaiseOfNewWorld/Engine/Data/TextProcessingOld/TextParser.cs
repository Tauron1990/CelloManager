using System.Collections.Immutable;
using RaiseOfNewWorld.Engine.Data.TextProcessing.Parsing;

namespace RaiseOfNewWorld.Engine.Data.TextProcessingOld;

public static class TextParser
{
    public static IEnumerable<ITextData> ParseText(string text)
    {
        var tokenizer = TokenizerOld.Tokens(text);

        while ( tokenizer.CanNext && tokenizer.Get().TokenType is not TokenType.Eof)
            yield return ReadFragment(tokenizer);
    }

    private static ITextData ReadFragment(TokenizerOld tokens)
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

    private static TextDataOld ReadMetadata(string name, TokenizerOld tokens)
    {
        var attributes = ImmutableArray<AttributeData>.Empty;
        string? type = null;

        if (tokens.Get().TokenType is TokenType.AttributeValueSeperator)
        {
            tokens.Incremnt();
            type = tokens.GetAndIncement().Text.ToLower();
        }

        ValidateToken(tokens, tokens.GetAndIncement(), TokenType.OpenAttribute);


        while (tokens.Get().TokenType is not TokenType.CloseAttribute or TokenType.Eof)
        {
            var attrName = ValidateToken(tokens, tokens.GetAndIncement(), TokenType.Text);
            ValidateToken(tokens, tokens.GetAndIncement(), TokenType.AttributeValueSeperator);
            var attrValue = ValidateToken(tokens, tokens.GetAndIncement(), TokenType.Text);

            attributes = attributes.Add(new AttributeData(attrName.Text.ToLower(), attrValue.Text.ToLower()));

            var token = tokens.GetAndIncement();
            if(token.TokenType is TokenType.CloseAttribute)
                break;
            if(token.TokenType is not TokenType.AttributeSeperator)
                ThrowInvalidToken(token, tokens);
        }
        
        if(tokens.Get().TokenType is TokenType.CloseAttribute)
            tokens.Incremnt();

        return new TextDataOld(name, type, attributes, ImmutableArray<ITextData>.Empty);
    }

    private static void ReadFragmentContent(ref TextDataOld dataOld, TokenizerOld tokens)
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
        dataOld = dataOld with { Content = textData };
    }

    private static TextToken ValidateToken(TokenizerOld tokens, TextToken textToken, TokenType expected)
    {
        if(textToken.TokenType != expected)
            ThrowInvalidToken(textToken, tokens);

        return textToken;
    }
    
    private static void ThrowInvalidToken(TextToken textToken, TokenizerOld tokenArray)
    {
        if(tokenArray.Pointer > 0)
            throw new InvalidOperationException($"Invalid Token: {textToken} after {tokenArray.GetPrevorius()}");
        throw new InvalidOperationException($"Invalid Token: {textToken}");
    }
}