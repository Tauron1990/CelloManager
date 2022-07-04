using System.Collections.Immutable;
using Terminal.Gui;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing;

public sealed record ViewContext(View Target, ImmutableDictionary<string, View> NamedViews);