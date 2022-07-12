using System.Collections.Immutable;
using Terminal.Gui;

namespace RaiseOfNewWorld.Engine.Data.TextProcessingOld;

public sealed record ViewContext(View Target, ImmutableDictionary<string, View> NamedViews)
{
    public View GetView(string name)
    {
        if (NamedViews.TryGetValue(name, out var view))
            return view;

        throw new InvalidOperationException($"No View with Name: {name} found");
    }
}