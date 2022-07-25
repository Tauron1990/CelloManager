using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq.Expressions;
using FastExpressionCompiler;
using Terminal.Gui;
using TextFragmentLib2.TextProcessing.Ast;

namespace TextFragmentLib2.TextProcessing.Templates;

public static class TemplateCompiler
{
    private static readonly ConcurrentDictionary<string, CompiledTemplate> Templates = new();

    public static CompiledTemplate GetTemplate(TemplateReferenceNode node)
        => Templates.GetOrAdd(
            node.Source,
            _ => new CompiledTemplate(node.Entrys.Select(CompileEntry).ToImmutableList()));

    private static CompiledTemplateEntry CompileEntry(TemplateEntryNode node)
    {
        var compiler = new TemplateMatchCompiler();
        var matcher = compiler.Accept(node.Match);

        return new CompiledTemplateEntry(
            Expression.Lambda<Func<View, bool>>(matcher, compiler.ParameterExpression).CompileFast(),
            node.Attributes);
    }
}