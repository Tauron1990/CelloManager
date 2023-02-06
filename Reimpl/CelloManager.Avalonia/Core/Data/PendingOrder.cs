using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CelloManager.Core.Logic;
using CelloManager.Core.Printing.Data;

namespace CelloManager.Core.Data;

public sealed record OrderedSpoolList(string Category, ImmutableList<OrderedSpool> Spools);

public sealed record OrderedSpool(string SpoolId, string Name, int Amount);

public sealed record PendingOrder(string Id, ImmutableList<OrderedSpoolList> Spools, DateTimeOffset Time) : IHasId
{
    public PrintPage ToPrintPage(IEnumerable<OrderedSpoolList> spools) => new(this with { Spools = spools.ToImmutableList() });
    
    public static PendingOrder New(IEnumerable<SpoolData> spools, Func<SpoolData, int> amountSelector)
    {

        return new(
            Guid.NewGuid().ToString("N"),
            ImmutableList<OrderedSpoolList>.Empty
                .AddRange(
                    spools
                        .GroupBy(s => s.Category)
                        .Select(
                            g => new OrderedSpoolList(
                                g.Key,
                                g.OrderBy(d => d.Name, ReadySpoolSorter.NameSorter)
                                    .Select(sd => new OrderedSpool(sd.Id, sd.Name, amountSelector(sd)))
                                    .ToImmutableList()))),
            DateTimeOffset.Now);
    }
}