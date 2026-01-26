using JetBrains.Annotations;

namespace CelloManager.Core.Printing.Workflow;

[PublicAPI]
public class ConditionConfiguration<TState, TContext>(
    StepConfiguration<TState, TContext> config,
    SimpleCondition<TContext> condition)
{
    public StepConfiguration<TState, TContext> GoesTo(StepId id)
    {
        condition.Target = id;

        return config;
    }
}