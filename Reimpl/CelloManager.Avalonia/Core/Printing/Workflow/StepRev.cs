using System.Collections.Generic;
using System.Text;

namespace CelloManager.Core.Printing.Workflow;

public sealed class StepRev<TState, TContext>(TState step)
{
    public TState Step { get; } = step;

    public IList<ICondition<TContext>> Conditions { get; } = new List<ICondition<TContext>>();

    public ICondition<TContext>? GenericCondition { get; set; }

    public override string ToString()
    {
        StringBuilder stringBuilder = new();
        stringBuilder.Append(Step);

        foreach (ICondition<TContext> condition in Conditions)
        {
            stringBuilder.AppendLine($"->{condition};");
        }

        if (GenericCondition != null)
        {
            stringBuilder.Append($"Generic->{GenericCondition};");
        }

        return stringBuilder.ToString();
    }
}