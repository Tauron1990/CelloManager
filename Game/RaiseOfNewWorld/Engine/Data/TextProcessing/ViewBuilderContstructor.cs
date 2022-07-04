using System.Collections.Immutable;
using Terminal.Gui;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing;

public static class ViewBuilderContstructor
{
    public static Func<ViewContext> CreateBuilder(ITextData textData)
        => textData switch
        {
            SimpleText simpleText => CreateSimpleBuilder(simpleText),
            TextData complexText => CreateComplexBuilder(complexText),
            _ => throw new InvalidOperationException($"Unkowen ITextData Types {textData}")
        };

    private static Func<ViewContext> CreateSimpleBuilder(SimpleText textData)
        => () => new ViewContext(new Label { Text = textData.Text }, ImmutableDictionary<string, View>.Empty);

    private static Func<ViewContext> CreateComplexBuilder(TextData textData)
        => textData.Type switch
        {
            _ => CreateView(static () => new View(), textData.Name),
        };

    private static Func<ViewContext> CreateView(Func<View> view, string? name) =>
        () =>
        {
            var viewInst = view();

            return string.IsNullOrWhiteSpace(name) 
                ? new ViewContext(viewInst, ImmutableDictionary<string, View>.Empty) 
                : new ViewContext(viewInst, ImmutableDictionary<string, View>.Empty.Add(name, viewInst));
        };
}