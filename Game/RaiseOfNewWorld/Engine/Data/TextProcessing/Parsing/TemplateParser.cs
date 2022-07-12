using System.Collections.Concurrent;
using RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Parsing;

//TODO Templates Parsing
public sealed class TemplateParser
{
    private static readonly ConcurrentDictionary<string, TemplateReferenceNode> _templates = new();

    private TemplateParser(string template)
    {
        
    }

    private TemplateReferenceNode ParseTemplate()
    {
        return new TemplateReferenceNode();
    }

    public static TemplateReferenceNode Parse(ContentManager contentManager, string fileName)
        => _templates.GetOrAdd(fileName,
            relativeFileName =>
                new TemplateParser(contentManager.ReadFile(relativeFileName)).ParseTemplate());
}