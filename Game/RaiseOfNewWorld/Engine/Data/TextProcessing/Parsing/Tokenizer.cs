using System.Collections.Immutable;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Parsing;

public sealed class Tokenizer : TokenizerBase<TextToken>
{
    private static ref TextToken CreateText(string? text, int position)
    {
        ref var textToken = new TextToken(text, TokenType.Text, position);
        return ref textToken;
    }
    
    private static 
    
    public Tokenizer(string input) 
        : base(input, tokens, CreateText)
    {
    }
}