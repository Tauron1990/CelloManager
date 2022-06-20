namespace RaiseOfNewWorld.Engine.Data;

public static class TextParser
{
    public static Func<IReadOnlyCollection<string>> Pages(Func<string> input) 
        => () => input().Split(new[] { "@@@" }, StringSplitOptions.TrimEntries);

    public static string FormatText(string text, ContentManager contentManager, string? filename)
    {
        
    }
}