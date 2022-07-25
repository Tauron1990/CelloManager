using System.Collections.Immutable;
using Terminal.Gui;
using TextFragmentLib2.TextProcessing.Ast;

namespace TextFragmentLib2.TextProcessing.Templates;

public sealed record CompiledTemplateEntry(Func<View, bool> IsMatch, ImmutableList<AttributeNode> Attributes);

public sealed record CompiledTemplate(ImmutableList<CompiledTemplateEntry> Entrys);