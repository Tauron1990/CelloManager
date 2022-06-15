namespace RaiseOfNewWorld.Engine.Data;

public static class TextParser
{
    public static IReadOnlyCollection<string> ParsePages(string input) => input.Split(new[] { "@@@" }, StringSplitOptions.TrimEntries);
}