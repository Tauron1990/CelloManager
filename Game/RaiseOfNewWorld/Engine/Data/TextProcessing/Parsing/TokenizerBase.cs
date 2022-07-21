using System.Buffers;
using System.Collections.Immutable;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Parsing;

public delegate TToken TokenBuilder<out TToken>(string text, int position)
    where TToken : struct;

public abstract class TokenizerBase<TToken>
    where TToken : struct
{
    private readonly TokenBuilder<TToken> _textBuilder;
    private readonly ImmutableDictionary<char, TokenBuilder<TToken>> _tokenBuilders;
    private string _input;
    private int _position;

    private ImmutableArray<TToken> _tokens = ImmutableArray<TToken>.Empty;

    protected TokenizerBase(string input, ImmutableDictionary<char, TokenBuilder<TToken>> tokens,
        TokenBuilder<TToken> textBuilder)
    {
        _input = input;
        _tokenBuilders = tokens;
        _textBuilder = textBuilder;

        NextToken();
    }

    protected int Pointer { get; private set; }

    public TToken Get()
    {
        if (Pointer == _tokens.Length)
            NextToken();
        return _tokens[Pointer];
    }

    public TToken GetAndIncement()
    {
        var token = Get();
        Pointer++;
        return token;
    }

    public void Incremnt()
    {
        Get();
        Pointer++;
    }

    protected TToken GetPrevorius()
        => _tokens[Pointer - 1];

    private void NextToken()
    {
        var text = _input.AsSpan();
        var token = NextToken(
            text,
            _position,
            out var lenght);
        if (lenght != 0)
        {
            _position += lenght;
            _input = text[lenght..].Trim().ToString();
        }

        _tokens = _tokens.Add(token);
    }

    private TToken NextToken(in ReadOnlySpan<char> text, int position, out int lenght)
    {
        if (text.IsEmpty)
        {
            lenght = 0;
            return _textBuilder(
                string.Empty,
                position);
        }

        for (var i = 0; i < text.Length; i++)
        {
            if (!_tokenBuilders.TryGetValue(
                    text[i],
                    out var builder)) continue;

            if (i == 0)
            {
                lenght = 1;
                return builder(
                    MakeString(text[..1]),
                    position);
            }

            lenght = i;
            return _textBuilder(
                MakeString(text[..i]),
                position);
        }

        lenght = text.Length;
        return _textBuilder(
            MakeString(text),
            position);
    }

    private static string MakeString(in ReadOnlySpan<char> text)
    {
        var arr = ArrayPool<char>.Shared.Rent(text.Length);
        try
        {
            var output = arr.AsSpan();
            var count = text.Trim().ToLower(
                output,
                null);

            return output[..count].ToString();
        }
        finally
        {
            ArrayPool<char>.Shared.Return(arr);
        }
    }
}