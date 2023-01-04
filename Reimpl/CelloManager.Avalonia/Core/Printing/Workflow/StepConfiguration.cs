using System;
using JetBrains.Annotations;

namespace CelloManager.Core.Printing.Workflow;

[PublicAPI]
public class StepConfiguration<TState, TContext>
{
    private readonly StepRev<TState, TContext> _context;

    public StepConfiguration(StepRev<TState, TContext> context) => _context = context;

    public StepConfiguration<TState, TContext> WithCondition(ICondition<TContext> condition)
    {
        _context.Conditions.Add(condition);

        return this;
    }

    public StepConfiguration<TState, TContext> WithCondition(Func<TContext, IStep<TContext>, bool>? guard, Action<ConditionConfiguration<TState, TContext>> config)
    {
        var con = new SimpleCondition<TContext> { Guard = guard };

        if(guard != null)
        {
            config(new ConditionConfiguration<TState, TContext>(WithCondition(con), con));
            return this;
        }

        _context.GenericCondition = con;

        config(new ConditionConfiguration<TState, TContext>(this, con));
        return this;
    }

    public StepConfiguration<TState, TContext> WithCondition(Action<ConditionConfiguration<TState, TContext>> config)
        => WithCondition(null, config);

}