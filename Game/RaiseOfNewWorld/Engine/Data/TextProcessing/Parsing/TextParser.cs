using System.Collections.Immutable;
using RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Parsing;

public sealed class TextParser
{
    private readonly string _input;
    private ContentManager _contentManager = ContentManager.Empty;

    public TextParser(string input)
    {
        _input = input;
    }

    public TextDataNode Parse(ContentManager contentManager)
    {
        _contentManager = contentManager;
        var tokens = new Tokenizer(_input);

        return new TextDataNode
        {
            Templates = ReadTemplates(tokens).ToImmutableList(),
            FragmentNodes = ReadFragments(tokens).ToImmutableList()
        };
    }

    private IEnumerable<TextFragmentNode> ReadFragments(Tokenizer tokenizer)
    {
        TextToken textToken;
        do
        {
            textToken = tokenizer.GetAndIncement();
            if (textToken.TokenType == TokenType.Text)
                yield return new TextFragmentNode { Text = textToken.Text };

            ValidateToken(
                textToken,
                TokenType.OpenFragment);
            textToken = tokenizer.Get();

            var fragment = new TextFragmentNode();

            if (textToken.TokenType == TokenType.Text)
            {
                tokenizer.Incremnt();

                fragment.Type = textToken.Text;

                textToken = tokenizer.Get();
                if (textToken.TokenType == TokenType.AttributeValueSeperator)
                {
                    tokenizer.Incremnt();
                    textToken = tokenizer.GetAndIncement();
                    ValidateToken(
                        textToken,
                        TokenType.Text);

                    fragment.Name = textToken.Text;
                }
            }

            fragment.Attributes = ReadAttributes(tokenizer).ToImmutableList();

            textToken = tokenizer.Get();
            if (textToken.TokenType == TokenType.Text)
            {
                tokenizer.Incremnt();
                fragment.Text = textToken.Text;
                textToken = tokenizer.Get();
            }

            if (textToken.TokenType == TokenType.OpenFragment)
            {
                fragment.FragmentNodes = ReadFragments(tokenizer).ToImmutableList();
                textToken = tokenizer.Get();
            }

            ValidateToken(
                textToken,
                TokenType.CloseFragment);
            tokenizer.Incremnt();

            yield return fragment;

            textToken = tokenizer.Get();
            if (textToken.TokenType != TokenType.CloseFragment) continue;

            yield break;
        } while (!textToken.IsEof);
    }

    public static IEnumerable<AttributeNode> ReadAttributes(Tokenizer tokenizer)
    {
        var textToken = tokenizer.GetAndIncement();
        ValidateToken(
            textToken,
            TokenType.OpenAttribute);

        do
        {
            textToken = tokenizer.GetAndIncement();
            if (textToken.TokenType == TokenType.CloseAttribute) yield break;

            var node = new AttributeNode();

            ValidateToken(
                textToken,
                TokenType.Text);
            node.Name = textToken.Text;

            textToken = tokenizer.GetAndIncement();
            ValidateToken(
                textToken,
                TokenType.AttributeValueSeperator);

            node.Value = ReadAttributeValue(tokenizer);

            textToken = tokenizer.GetAndIncement();
            if (textToken.TokenType is TokenType.AttributeSeperator or TokenType.CloseAttribute)
            {
                yield return node;
                if (textToken.TokenType == TokenType.CloseAttribute)
                    yield break;
            }
            else
            {
                ThrowInvalidToken(textToken);
            }
        } while (!textToken.IsEof);
    }

    private static AttributeValueNode ReadAttributeValue(Tokenizer tokenizer)
    {
        var token = tokenizer.GetAndIncement();
        ValidateToken(
            token,
            TokenType.Text);

        var token2 = tokenizer.Get();
        if (token2.TokenType is TokenType.AttributeSeperator or TokenType.CloseAttribute)
            return CreateAttributeTextValue(token.Text);

        AttributeValueNode left;
        if (token2.TokenType is TokenType.OpenAttribute)
        {
            tokenizer.Incremnt();
            left = new CallAttributeValue
            {
                MethodName = token.Text,
                Parameters = ReadParameters(tokenizer).ToImmutableList()
            };

            token2 = tokenizer.Get();
        }
        else
        {
            left = CreateAttributeTextValue(token.Text);
        }

        return token2.TokenType switch
        {
            TokenType.AttributeSeperator or TokenType.CloseAttribute => left,
            TokenType.Plus or TokenType.Minus => CreateExpression(),
            _ => throw CreateInvalidToken(token2)
        };

        ExpressionAttributeValue CreateExpression()
        {
            tokenizer.Incremnt();

            return new ExpressionAttributeValue
            {
                OperatorType = token2.TokenType == TokenType.Plus ? OperatorType.Add : OperatorType.Subtract,
                Left = left,
                Right = ReadAttributeValue(tokenizer)
            };
        }
    }

    private static TextAttributeValue CreateAttributeTextValue(string value)
        => value.StartsWith('@')
            ? new TextAttributeValue { IsReference = true, Value = value[1..] }
            : new TextAttributeValue { Value = value };

    private static IEnumerable<AttributeValueNode> ReadParameters(Tokenizer tokenizer)
    {
        do
        {
            yield return ReadAttributeValue(tokenizer);

            var token = tokenizer.GetAndIncement();
            if (token.TokenType == TokenType.CloseAttribute) yield break;

            ValidateToken(
                token,
                TokenType.AttributeSeperator);
        } while (true);
    }

    private IEnumerable<TemplateReferenceNode> ReadTemplates(Tokenizer tokenizer)
    {
        while (true)
        {
            var token = tokenizer.Get();
            if (token.TokenType == TokenType.Template)
                yield return ReadTemplate(tokenizer);
            else
                yield break;
        }
    }

    private TemplateReferenceNode ReadTemplate(Tokenizer tokenizer)
    {
        tokenizer.Incremnt();
        var textRef = tokenizer.GetAndIncement();
        ValidateToken(
            textRef,
            TokenType.Text);

        return TemplateParser.Parse(
            _contentManager,
            textRef.Text);
    }

    private static void ValidateToken(TextToken textToken, TokenType expected)
    {
        if (textToken.TokenType == expected)
            return;

        ThrowInvalidToken(
            textToken,
            expected);
    }

    private static void ThrowInvalidToken(TextToken textToken, TokenType expected)
    {
        throw new InvalidOperationException($"Invalid Token {textToken} :Expeced:{expected}");
    }

    private static void ThrowInvalidToken(TextToken textToken)
    {
        throw new InvalidOperationException($"Invalid Token {textToken}");
    }

    private static InvalidOperationException CreateInvalidToken(TextToken textToken)
        => new($"Invalid Token {textToken}");
}