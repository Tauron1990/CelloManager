using JetBrains.Annotations;

namespace CelloManager.Core.Printing.Workflow;

[PublicAPI]
public class ConditionConfiguration<TState, TContext>
{
    private readonly SimpleCondition<TContext> _condition;
    private readonly StepConfiguration<TState, TContext> _config;

    public ConditionConfiguration(
        StepConfiguration<TState, TContext> config,
        SimpleCondition<TContext> condition)
    {
        _config = config;
        _condition = condition;
    }

    public StepConfiguration<TState, TContext> GoesTo(StepId id)
    {
        _condition.Target = id;

        return _config;
    }
}