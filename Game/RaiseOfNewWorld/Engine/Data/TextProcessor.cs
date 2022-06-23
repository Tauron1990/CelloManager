using System.Text;
using Microsoft.Extensions.ObjectPool;
using SystemsRx.Pools;
using Terminal.Gui;

namespace RaiseOfNewWorld.Engine.Data;

public static class TextProcessor
{
    private static readonly ObjectPool<List<TextFragment>> FragmentPools = ObjectPool.Create(new DefaultPooledObjectPolicy<List<TextFragment>>());

    //private static readonly ObjectPool<StringBuilder> StringBuilderPools = ObjectPool.Create(new DefaultPooledObjectPolicy<StringBuilder>());
    
    public static Func<IReadOnlyCollection<string>> Pages(Func<string> input) 
        => () => input().Split(new[] { "@@@" }, StringSplitOptions.TrimEntries);

    public static TextFragment FormatText(string text, ContentManager contentManager, string? filename)
    {
        var span = ("{" + text + "}").AsSpan();
        var pointer = 0;
        
        return ParseText(span, ref pointer, contentManager, filename);
    }

    // ReSharper disable once CognitiveComplexity
    private static TextFragment ParseText(in ReadOnlySpan<char> text, ref int pointer, ContentManager contentManager, string? filename)
    {
        pointer++;
        
        var fragments = FragmentPools.Get();
        var start = 0;
        var attributeBuilder = ReadAttributes(text, ref pointer);
        
        try
        {
            while (pointer < text.Length)
            {
                if (text[pointer] == '{')
                {
                    pointer++;
                    if (start != 0)
                        fragments.Add(ExtractFragment(text, pointer));
                    fragments.Add(ParseText(text, ref pointer, contentManager, filename));
                    start = 0;
                }
                else if (text[pointer] == '}')
                {
                    pointer++;
                    break;
                }
                else
                {
                    start++;
                    pointer++;
                }
            }
            
            if(start != 0)
                fragments.Add(ExtractFragment(text, pointer));
            
            return fragments.Count > 0 ? TextFragment.Create(fragments) : fragments[0];
        }
        finally
        {
            FragmentPools.Return(fragments);
        }

        TextFragment ExtractFragment(in ReadOnlySpan<char> text, int pointer)
            => TextFragment.Create(text.Slice(pointer, start).ToString(), attributeBuilder);
    }

    private static Action<Label>? ReadAttributes(in ReadOnlySpan<char> text, ref int pointer)
    {
        while (text.Length > pointer)
        {
            if (text[pointer] == ')')
            {
                pointer++;
                break;
            }
        }

        return null;
    }
}