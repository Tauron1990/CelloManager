using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using CelloManager.Core.Data;
using CelloManager.Core.Printing.Workflow;

namespace CelloManager.Core.Printing.Steps;

public sealed class CollectInfoStep : PrinterStep
{
    public override ValueTask<StepId> OnExecute(PrinterContext context)
    {
        var order = GetOrThrow(context, static c => c.Order);

        var pageSpools = new List<OrderedSpoolList>();
        
        foreach (var orderedSpoolList in order.Spools)
        {
            if (orderedSpoolList.Spools.Count > 20)
                SplitList(orderedSpoolList, pageSpools);
            else
                pageSpools.Add(orderedSpoolList);

            while (pageSpools.Count >= 2)
            {
                context.Pages = context.Pages.Add(order.ToPrintPage(pageSpools.Take(2)));
                pageSpools.RemoveRange(0, 2);
            }
        }

        if (pageSpools.Count != 0)
            context.Pages = context.Pages.Add(order.ToPrintPage(pageSpools));

        return ValueTask.FromResult(StepId.None);
    }

    private static void SplitList(OrderedSpoolList orderedSpoolList, ICollection<OrderedSpoolList> pageSpools)
    {
        var spools = orderedSpoolList.Spools;
        while (spools.Count > 20)
        {
            pageSpools.Add(orderedSpoolList with { Spools = spools.Take(20).ToImmutableList() });
            spools = spools.RemoveRange(0, 20);
        }

        if (spools.Count != 0)
            pageSpools.Add(orderedSpoolList with { Spools = spools });
    }

    //OLD System
    /*var order = GetOrThrow(context, static c => c.Order);
if(order.Spools.Count <= 2)
{
    context.AddPage(new PrintPage(order));
}
else
{
    var toProcess = order.Spools.ToList();
    var inProcess = new List<OrderedSpoolList>();

    while (toProcess.Count != 0)
        ProcessGroup(context, toProcess, inProcess, order);

    if(inProcess.Count != 0)
        context.Pages = context.Pages.Add(order.ToPrintPage(inProcess));
}

return ValueTask.FromResult(StepId.None);*/
    
    /*private static void ProcessGroup(PrinterContext context, ICollection<OrderedSpoolList> toProcess, List<OrderedSpoolList> inProcess, PendingOrder order)
    {
        var next = toProcess.Take(2).ToArray();

        if(inProcess.Count == 0)
        {
            inProcess.AddRange(next);
            foreach (var spoolList in next)
                toProcess.Remove(spoolList);
            
            return;
        }
        
        for (var i = 0; i < next.Length; i++)
        {
            var inProcessElement = inProcess[i];
            var nextElement = next[i];

            if (inProcessElement.Spools.Count + nextElement.Spools.Count <= 15) continue;
            
            context.AddPage(order.ToPrintPage(inProcess));
            inProcess.Clear();
            break;
        }

        foreach (var spoolList in next)
            toProcess.Remove(spoolList);
        inProcess.AddRange(next);
    }*/
}