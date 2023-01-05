using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using CelloManager.Core.Data;
using CelloManager.Core.Printing.Impl;
using CelloManager.Core.Printing.Steps;
using CelloManager.Core.Printing.Workflow;
using Microsoft.Extensions.DependencyInjection;

namespace CelloManager.Core.Printing;

public sealed class PrintBuilder
{
   

    public async ValueTask PrintPendingOrder(PendingOrder order, Dispatcher dispatcher, IServiceProvider serviceProvider, Action? end)
    {
        var errors = serviceProvider.GetRequiredService<ErrorDispatcher>();
        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        
        try
        {
            var workflow = scope.ServiceProvider.GetRequiredService<PrinterWorkflow>();
            var printerContext = scope.ServiceProvider.GetRequiredService<PrinterContext>();

            printerContext.Dispatcher = dispatcher;
            printerContext.ServiceProvider = scope.ServiceProvider;
            printerContext.Order = order;
            printerContext.End = end;
            
            await workflow.Begin(StepId.Start, printerContext);
        }
        catch (Exception e)
        {
            errors.Send(e);
        }
    }
}