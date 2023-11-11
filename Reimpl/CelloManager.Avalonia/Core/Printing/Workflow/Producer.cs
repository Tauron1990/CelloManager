using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace CelloManager.Core.Printing.Workflow;

[PublicAPI]
public abstract class Producer<TSelf, TState, TContext>
    where TState : IStep<TContext>
    where TContext : notnull
    where TSelf : Producer<TSelf, TState, TContext>
{
    private readonly Dictionary<StepId, StepRev<TState, TContext>> _states = new();

    private string _errorMessage = string.Empty;

    private StepId _lastId;

    public async ValueTask Begin(StepId id, TContext context)
    {
        if(!await Process(id, context).ConfigureAwait(false))
            throw new InvalidOperationException("Procession not Successful");

        if(string.Equals(_lastId.Name, StepId.Fail.Name, StringComparison.Ordinal))
            throw new InvalidOperationException(_errorMessage);
    }

    [DebuggerStepThrough]
    protected bool SetLastId(StepId id)
    {
        _lastId = id;

        return string.Equals(_lastId.Name, StepId.Finish.Name, StringComparison.Ordinal)
            || string.Equals(_lastId.Name, StepId.Fail.Name, StringComparison.Ordinal);
    }

    protected virtual async ValueTask<bool> Process(StepId id, TContext context)
    {
        if(SetLastId(id)) return true;

        if(!_states.TryGetValue(id, out var rev))
            return SetLastId(StepId.Fail);

        StepId sId = await rev.Step.OnExecute(context).ConfigureAwait(false);
        var result = false;

        switch (sId.Name)
        {
            case "Fail":
                _errorMessage = rev.Step.ErrorMessage;

                return SetLastId(sId);
            case "None":
                result = await ProgressConditions(rev, context).ConfigureAwait(false);

                break;
            case "Loop":
                (bool isOk, bool loopResult) = await ProcessLoop(context, rev).ConfigureAwait(false);
                if(isOk)
                    return loopResult;

                break;
            case "Finish":
            case "Skip":
                result = true;

                break;
            default:
                return SetLastId(StepId.Fail);
        }

        if(!result)
            await rev.Step.OnExecuteFinish(context).ConfigureAwait(false);

        return result;
    }

    private async ValueTask<(bool IsOk, bool Result)> ProcessLoop(TContext context, StepRev<TState, TContext> rev)
    {
        var ok = true;

        do
        {
            StepId loopId = await rev.Step.NextElement(context).ConfigureAwait(false);
            if(string.Equals(loopId.Name, StepId.LoopEnd.Name, StringComparison.Ordinal))
            {
                ok = false;

                continue;
            }

            if(string.Equals(loopId.Name, StepId.Fail.Name, StringComparison.Ordinal))
                return (true, SetLastId(StepId.Fail));

            await ProgressConditions(rev, context).ConfigureAwait(false);
        } while (ok);

        return (false, true);
    }

    private async ValueTask<bool> ProgressConditions(StepRev<TState, TContext> rev, TContext context)
    {
        var std = (from con in rev.Conditions
                   let stateId = con.Select(rev.Step, context)
                   where !string.Equals(stateId.Name, StepId.None.Name, StringComparison.Ordinal)
                   select stateId).ToArray();

        if(std.Length != 0)
        {
            foreach (StepId stepId in std)
            {
                if(await Process(stepId, context).ConfigureAwait(false))
                    return true;
            }
            return false;
        }

        if(rev.GenericCondition is null) return false;

        StepId cid = rev.GenericCondition.Select(rev.Step, context);

        return !string.Equals(cid.Name, StepId.None.Name, StringComparison.Ordinal) && await Process(cid, context).ConfigureAwait(false);
    }

    
    protected TSelf SetStep(StepId id, TState stade, Action<StepConfiguration<TState, TContext>> config)
    {
        var rev = new StepRev<TState, TContext>(stade);
        _states[id] = rev;

        config(new StepConfiguration<TState, TContext>(rev));

        return (TSelf)this;
    }
}