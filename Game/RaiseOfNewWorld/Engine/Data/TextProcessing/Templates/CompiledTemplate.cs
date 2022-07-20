using RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;
using SystemsRx.MicroRx;
using Terminal.Gui;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Templates;

public sealed record CompiledTemplate(Func<View, bool> IsMatch, ImmutableList<AttributeNode> Attributes);