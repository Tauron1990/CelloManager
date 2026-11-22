using CelloManager.Core.Data;
using CelloManager.Core.Printing.Internal;
using CelloManager.Core.Printing.Steps;
using Scriban;
using Scriban.Runtime;

const string test =
    """
    Grüß Gott,
    
    Bitte:
    
    {{~ for spools in order.spools ~}}
    {{spools.category}}:
    {{~ for spool in spools.spools ~}}
        {{spool.name}}:{{spool.amount}}
    {{~ end ~}}
    {{~ end ~}}
    
    Danke
    """;


PendingOrder testOrder = PendingOrder.New(
    GenerateSpoolData(), 
    sd => sd.NeedAmount);

PrinterContext context = new(new PrintUiModel())
                         {
                             Order = testOrder,
                         };

CollectInfoStep collectInfoStep = new();
await collectInfoStep.OnExecute(context);

Template testTemplate = Template.Parse(test);
TemplateContext templateContext = ConfigurateContext(context);

string result = testTemplate.Render(templateContext);

Console.WriteLine(result);
return;

TemplateContext ConfigurateContext(PrinterContext printerContext)
{
    ScriptObject scriptObject = new()
                                {
                                    { "order", printerContext.Order },
                                    { "pages", printerContext.Pages },
                                    { "uimodel", printerContext.PrintUiModel },
                                };

    return new TemplateContext(scriptObject);
}

IEnumerable<SpoolData> GenerateSpoolData()
{
    for (int i = 0; i < 5; i++)
    {
        string cat;
        
        if (i > 2)
        {
            cat = "Glanz";
        }
        else
        {
            cat = "Matt";
        }

        yield return new SpoolData(Guid.NewGuid().ToString(), i.ToString(), cat, i, i + 4);
    }
}