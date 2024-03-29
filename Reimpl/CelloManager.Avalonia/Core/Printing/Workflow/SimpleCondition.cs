﻿using System;
using JetBrains.Annotations;

namespace CelloManager.Core.Printing.Workflow;

[PublicAPI]
public class SimpleCondition<TContext> : ICondition<TContext>
{
    public Func<TContext, IStep<TContext>, bool>? Guard { get; set; }

    public StepId Target { get; set; } = StepId.None;

    public StepId Select(IStep<TContext> lastStep, TContext context)
    {
        if(Guard is null || Guard(context, lastStep)) return Target;

        return StepId.None;
    }

    public override string ToString() => $"Target: {Target}";
}