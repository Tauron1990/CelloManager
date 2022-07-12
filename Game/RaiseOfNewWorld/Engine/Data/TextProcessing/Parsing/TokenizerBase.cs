using System.Buffers;
using System.Collections.Immutable;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Parsing;

public delegate ref TToken TokenBuilder<TToken>(string? text, int position)
    where TToken : struct;

public abstract class TokenizerBase<TToken>
    where TToken : struct
{
    private readonly ImmutableDictionary<char, TokenBuilder<TToken>> _tokenBuilders;
    private readonly TokenBuilder<TToken> _textBuilder;
    
    private ImmutableArray<TToken> _tokens = ImmutableArray<TToken>.Empty;
    private int _position;
    private string _input;
    
    protected int Pointer { get; private set; }

    protected TokenizerBase(string input, ImmutableDictionary<char, TokenBuilder<TToken>> tokens, TokenBuilder<TToken> textBuilder)
    {
        _input = input;
        _tokenBuilders = tokens;
        _textBuilder = textBuilder;

        NextToken();
    }

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


    private void NextToken()
    {
        var text = _input.AsSpan();
        var token = NextToken(text, _position, out var lenght);
        if (lenght != 0)
        {
            _position += lenght;
            _input = text[lenght..].Trim().ToString();
        }

        _tokens = _tokens.Add(token);
    }

    private ref TToken NextToken(in ReadOnlySpan<char> text, int position, out int lenght)
    {
        if (text.IsEmpty)
        {
            lenght = 0;
            return ref _textBuilder(null, position);
        }

        for (var i = 0; i < text.Length; i++)
        {
            if (!_tokenBuilders.TryGetValue(text[i], out var builder)) continue;
            
            if (i == 0)
            {
                lenght = 1;
                return ref builder(MakeString(text[..1]), position);
            }

            lenght = i + 1;
            return ref _textBuilder(MakeString(text[..i]), position);
        }

        lenght = text.Length;
        return ref _textBuilder(MakeString(text), position);
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
}