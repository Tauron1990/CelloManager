using System;
using Scriban;
using Scriban.Runtime;

namespace CelloManager.Core.Printing.Steps;

public static class TextGenerator
{
    private static readonly Lazy<Template> DefaultTemplate = new(
        () =>
            Template.Parse("""
                           Grüß Gott,

                           Bitte:

                           {{~ for spools in order.spools ~}}
                           {{spools.category}}:
                           {{~ for spool in spools.spools ~}}
                               {{spool.name}}:{{spool.amount}}
                           {{~ end ~}}
                           {{~ end ~}}

                           Danke
                           """));

    private static TemplateContext ConfigurateContext(PrinterContext printerContext)
    {
        ScriptObject scriptObject = new(StringComparer.Ordinal)
                                    {
                                        { "order", printerContext.Order },
                                        { "pages", printerContext.Pages },
                                        { "uimodel", printerContext.PrintUiModel },
                                    };

        return new TemplateContext(scriptObject, StringComparer.Ordinal);
    }
    
    public static string RenderOrder(PrinterContext context, Template? template = null)
    {
        template ??= DefaultTemplate.Value;
        TemplateContext templateContext = ConfigurateContext(context);
        return template.Render(templateContext);
    }
}