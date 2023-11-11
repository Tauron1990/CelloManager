using JetBrains.Annotations;

namespace CelloManager.Core.Printing.Workflow;

[PublicAPI]
public readonly record struct StepId(string Name )
{
    //public static readonly StepId Null = new StepId();

    public static readonly StepId Fail = new("Fail");
    public static readonly StepId None = new("None");
    public static readonly StepId Finish = new("Finish");
    public static readonly StepId Loop = new("Loop");
    public static readonly StepId LoopEnd = new("LoopEnd");
    public static readonly StepId LoopContinue = new("LoopContinue");
    public static readonly StepId Skip = new("Skip");
    public static readonly StepId Start = new("Start");
    public static readonly StepId Waiting = new("Waiting");
}