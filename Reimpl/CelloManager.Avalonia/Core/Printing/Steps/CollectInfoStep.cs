using System.Threading.Tasks;
using CelloManager.Core.Printing.Workflow;

namespace CelloManager.Core.Printing.Steps;

public sealed class CollectInfoStep : PrinterStep
{
    public override ValueTask<StepId> OnExecute(PrinterContext context)
    {
        return base.OnExecute(context);
    }
}