using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CelloManager.Core.Data;
using CelloManager.Core.Printing.Data;
using CelloManager.Core.Printing.Workflow;

namespace CelloManager.Core.Printing.Steps;

public sealed class CollectInfoStep : PrinterStep
{
    public override ValueTask<StepId> OnExecute(PrinterContext context)
    {
        PendingOrder order = GetOrThrow(context, static c => c.Order);
        if(order.Spools.Count <= 3)
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
        
        return ValueTask.FromResult(StepId.None);
    }

    private static void ProcessGroup(PrinterContext context, List<OrderedSpoolList> toProcess, List<OrderedSpoolList> inProcess, PendingOrder order)
    {
        var next = toProcess.Take(3).ToArray();

        if(inProcess.Count == 0)
        {
            inProcess.AddRange(next);
            foreach (var spoolList in next)
                toProcess.Remove(spoolList);
            
            return;
        }
        
        for (int i = 0; i < next.Length; i++)
        {
            OrderedSpoolList inProcessElement = inProcess[i];
            OrderedSpoolList nextElement = next[i];

            if(inProcessElement.Spools.Count + nextElement.Spools.Count > 15)
            {
                context.AddPage(order.ToPrintPage(inProcess));
                inProcess.Clear();
                break;
            }
        }

        foreach (var spoolList in next)
            toProcess.Remove(spoolList);
        inProcess.AddRange(next);
    }
}