using RaiseOfNewWorld.Engine.Data.TextProcessing;

namespace RaiseOfNewWorld.Engine.Data;

public static class TextProcessor
{
    //private static readonly ObjectPool<StringBuilder> StringBuilderPools = ObjectPool.Create(new DefaultPooledObjectPolicy<StringBuilder>());

    public static Func<IReadOnlyCollection<string>> Pages(Func<string> input)
        => () => input().Split(
            new[] { "@@@" },
            StringSplitOptions.TrimEntries);

    public static TextFragment FormatText(string text)
        => FragmentFactory.Create(text);
}