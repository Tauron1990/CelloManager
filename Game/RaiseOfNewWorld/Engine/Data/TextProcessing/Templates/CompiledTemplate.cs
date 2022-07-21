using System.Collections.Immutable;
using RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;
using Terminal.Gui;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Templates;

public sealed record CompiledTemplateEntry(Func<View, bool> IsMatch, ImmutableList<AttributeNode> Attributes);

public sealed record CompiledTemplate(ImmutableList<CompiledTemplateEntry> Entrys);