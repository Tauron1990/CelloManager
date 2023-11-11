using System.Threading.Tasks;
using CelloManager.Core.Printing.Workflow;

namespace CelloManager.Core.Printing.Steps;

public sealed class CleanUpStep : PrinterStep
{
    public override ValueTask<StepId> OnExecute(PrinterContext context)
    {
        context.PrintDocument?.Dispose();
        
        return ValueTask.FromResult(StepId.None);
    }
}