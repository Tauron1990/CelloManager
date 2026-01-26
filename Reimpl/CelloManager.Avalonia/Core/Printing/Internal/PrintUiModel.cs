using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CelloManager.Core.Data;
using CelloManager.Core.Printing.Data;
using CelloManager.Core.Printing.Steps;

namespace CelloManager.Core.Printing.Internal;

public sealed class PrintUiModel : IEnumerable<PendingOrder>
{
    private readonly List<PrintPage> _pages = [];

    public IEnumerator<PendingOrder> GetEnumerator() => _pages.Select(p => p.SeperatedOrder).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public ValueTask Init(PrinterContext context)
    {
        foreach (PrintPage printPage in context.Pages)
        {
            _pages.Add(printPage);
        }

        return ValueTask.CompletedTask;
    }
}