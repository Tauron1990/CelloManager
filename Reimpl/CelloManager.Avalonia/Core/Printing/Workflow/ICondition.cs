namespace CelloManager.Core.Printing.Workflow;

public interface ICondition<TContext>
{
    StepId Select(IStep<TContext> lastStep, TContext context);
}