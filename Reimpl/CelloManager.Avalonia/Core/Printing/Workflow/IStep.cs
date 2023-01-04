using System.Threading.Tasks;

namespace CelloManager.Core.Printing.Workflow;

public interface IStep<in TContext>
{
    string ErrorMessage { get; }

    //StepId Id { get; }

    ValueTask<StepId> OnExecute(TContext context);

    ValueTask<StepId> NextElement(TContext context);

    ValueTask OnExecuteFinish(TContext context);
}