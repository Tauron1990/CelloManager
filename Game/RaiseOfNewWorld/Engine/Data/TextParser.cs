namespace RaiseOfNewWorld.Engine.Data;

public static class TextParser
{
    public static Func<IReadOnlyCollection<string>> Pages(Func<string> input) 
        => () => input().Split(new[] { "@@@" }, StringSplitOptions.TrimEntries);
}