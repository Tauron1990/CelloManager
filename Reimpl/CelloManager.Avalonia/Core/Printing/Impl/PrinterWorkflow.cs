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
            configuration =>
            {

            });
    }
}