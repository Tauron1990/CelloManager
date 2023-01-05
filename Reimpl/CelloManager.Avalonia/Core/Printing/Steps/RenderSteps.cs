using CelloManager.Core.Printing.Workflow;

namespace CelloManager.Core.Printing.Steps;

public static class RenderSteps
{
    public static readonly StepId RenderImages = new(nameof(RenderImages));

    public static readonly StepId CleanUp = new(nameof(CleanUp));

    public static readonly StepId DocumentSelector = new(nameof(DocumentSelector));

    public static readonly StepId DoPrint = new(nameof(DoPrint));
}