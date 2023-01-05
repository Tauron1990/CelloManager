using CelloManager.Core.Printing.Steps;
using CelloManager.Core.Printing.Workflow;

namespace CelloManager.Core.Printing.Impl;

public class PrinterWorkflow : Producer<PrinterWorkflow, PrinterStep, PrinterContext>
{
    public PrinterWorkflow()
    {
        SetStep(
            StepId.Start,
            new CollectInfoStep(),
            configuration => configuration.WithCondition(c => c.GoesTo(RenderSteps.DocumentSelector)));

        SetStep(
            RenderSteps.DocumentSelector,
            new DocumentTypeSelector(),
            c => c.WithCondition(cc => cc.GoesTo(RenderSteps.RenderImages)));

        SetStep(
            RenderSteps.RenderImages,
            new PrerenderImages(),
            c => c.WithCondition(cc => cc.GoesTo(RenderSteps.DoPrint)));

        SetStep(
            RenderSteps.DoPrint,
            new DoPrint(),
            c => c.WithCondition(cc => cc.GoesTo(RenderSteps.CleanUp)));
        
        SetStep(
            RenderSteps.CleanUp,
            new CleanUpStep(),
            c => c.WithCondition(cc => cc.GoesTo(StepId.Finish)));
    }
}