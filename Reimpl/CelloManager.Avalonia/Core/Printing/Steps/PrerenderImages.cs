using System.Threading.Tasks;
using CelloManager.Core.Printing.Impl;
using CelloManager.Core.Printing.Internal;
using CelloManager.Core.Printing.Workflow;
using CelloManager.Views.Orders;

namespace CelloManager.Core.Printing.Steps;

public sealed class PrerenderImages : PrinterStep
{
    public override async ValueTask<StepId> OnExecute(PrinterContext context)
    {
        await context.PrintUiModel.Init(context);
        
        PrintUiModel model = context.PrintUiModel;

        while(model.PendingCount != 0)
        {
            var count = model.PendingCount;
            
            PendingOrderPrintView view = await model.Next();

            await Task.Delay(200);

            view.RenderTo(context.TempFiles.GetAndCacheTempFile(count).WriteStream);
        };
        
        model.Dispose();
        
        return StepId.None;
    }
}