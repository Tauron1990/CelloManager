using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using CelloManager.Core.Data;
using CelloManager.Core.Printing.Impl;
using CelloManager.Core.Printing.Steps;
using CelloManager.Core.Printing.Workflow;
using Microsoft.Extensions.DependencyInjection;

namespace CelloManager.Core.Printing;

public static class PrintBuilder
{
    public static async ValueTask PrintPendingOrder(PendingOrder order, Dispatcher dispatcher, IServiceProvider serviceProvider, Action? end)
    {
        ErrorDispatcher errors = serviceProvider.GetRequiredService<ErrorDispatcher>();
        AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        await using (scope.ConfigureAwait(false))
        {
            try
            {
                PrinterWorkflow workflow = scope.ServiceProvider.GetRequiredService<PrinterWorkflow>();
                PrinterContext printerContext = scope.ServiceProvider.GetRequiredService<PrinterContext>();

                printerContext.Dispatcher = dispatcher;
                printerContext.ServiceProvider = scope.ServiceProvider;
                printerContext.Order = order;
                printerContext.End = end;

                await workflow.Begin(StepId.Start, printerContext).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                errors.Send(e);
            }
        }
    }
}