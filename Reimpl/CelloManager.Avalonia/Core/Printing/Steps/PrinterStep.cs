using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CelloManager.Core.Printing.Workflow;
using JetBrains.Annotations;

namespace CelloManager.Core.Printing.Steps;

[PublicAPI]
public abstract class PrinterStep : IStep<PrinterContext>
{
    public string ErrorMessage { get; protected set; } = string.Empty;

    public virtual ValueTask<StepId> OnExecute(PrinterContext context)
        => ValueTask.FromResult(StepId.Fail);

    public virtual ValueTask<StepId> NextElement(PrinterContext context)
        => ValueTask.FromResult(StepId.Fail);

    public virtual ValueTask OnExecuteFinish(PrinterContext context) => ValueTask.CompletedTask;

    protected static TType GetOrThrow<TType>(
        PrinterContext context, Func<PrinterContext, TType?> accessor,
        [CallerArgumentExpression(nameof(accessor))]
        string? expession = null)
    {
        TType? result = accessor(context);
        return result ?? throw new InvalidOperationException($"Element from \"{expession}\" is null");
    }
}