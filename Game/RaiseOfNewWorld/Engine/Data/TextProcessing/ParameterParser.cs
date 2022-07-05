using System.Collections.Immutable;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing;

public sealed class ParameterParser
{
    public string MethodName { get; }

    private ImmutableArray<string> Parameters { get; } = ImmutableArray<string>.Empty;
    
    public ParameterParser(string token)
    {
        ReadOnlySpan<char> tokenSpan = token;
        var index = tokenSpan.IndexOf('_');
        if (index == -1)
        {
            MethodName = token;
            return;
        }

        MethodName = token[..index].ToLower();
        tokenSpan = tokenSpan[(index + 1)..];
        
        do
        {
            index = tokenSpan.IndexOf('_');
            if (index == -1)
                Parameters = Parameters.Add(tokenSpan.ToString());
            else
            {
                Parameters = Parameters.Add(tokenSpan[..index].ToString());
                tokenSpan = tokenSpan[(index + 1)..];
            }
            
        } while (index != -1);
    }

    public bool CanResolve(int pos)
        => Parameters.Length > pos;
    
    public string ResolveParameter(int pos)
        => Parameters[pos];
    
    public TResult ResolveParameter<TResult>(int pos, Func<string, TResult> parser)
        => parser(pos == -1 ? MethodName : Parameters[pos]);
}