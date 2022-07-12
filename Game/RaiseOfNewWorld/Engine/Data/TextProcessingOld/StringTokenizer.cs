using System.Collections.Immutable;

namespace RaiseOfNewWorld.Engine.Data.TextProcessingOld;

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